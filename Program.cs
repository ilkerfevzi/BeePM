using BeePM.Services;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.EntityFrameworkCore.SqlServer;
using Elsa.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddSingleton<ApprovalLogService>();

builder.Services.AddDbContext<BeePM.Models.BeePMDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BeePM")));

// Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseAuthentication(); // <-- ekledik
app.UseAuthorization();


// Elsa runtime + HTTP aktiviteleri
builder.Services.AddElsa(elsa =>
{
    elsa.AddWorkflowsFrom<Program>();
    elsa.UseHttp(http => http.ConfigureHttpOptions = options =>
    {
        options.BaseUrl = new Uri("https://localhost:7192");
        options.BasePath = "/workflows";
    });

    var cs = builder.Configuration.GetConnectionString("BeePM"); 
    // Workflow Definition & Instance kalıcılığı (EF + SQL Server)
    elsa.UseWorkflowManagement(m => m.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));
    elsa.UseWorkflowRuntime(r => r.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));
     

}); 

// Elsa için HttpClient
builder.Services.AddHttpClient("Elsa", client =>
{
    // 📌 Lokalde çalışırken bu portu kullan
    client.BaseAddress = new Uri("https://localhost:7192");

    // 📌 IIS’e deploy edince burayı güncelle:
    // client.BaseAddress = new Uri("https://intranet.firma.com/BeePM");
});
 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// Elsa HTTP endpoint’lerini devreye al
app.UseWorkflows();




app.Run();
