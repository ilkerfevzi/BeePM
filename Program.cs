using BeePM.Services; 
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.EntityFrameworkCore.SqlServer;
using Elsa.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Elsa.Http;
using Elsa.EntityFrameworkCore.Extensions; 


var builder = WebApplication.CreateBuilder(args);
// Elsa runtime + HTTP aktiviteleri
builder.Services.AddElsa(elsa =>
{
    elsa.AddWorkflowsFrom<Program>();

    // Elsa HTTP
    elsa.UseHttp(http => http.ConfigureHttpOptions = options =>
    {
        options.BaseUrl = new Uri("https://localhost:7192");
        options.BasePath = "/workflows";
    });

    // Connection string
    var cs = builder.Configuration.GetConnectionString("BeePM");

    // Workflow Definition & Instance kalıcılığı (EF + SQL Server)
    elsa.UseWorkflowManagement(m => m.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));
    elsa.UseWorkflowRuntime(r => r.UseEntityFrameworkCore(ef => ef.UseSqlServer(cs)));
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
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

builder.Services.AddHttpClient("Elsa", client =>
{
    client.BaseAddress = new Uri("https://localhost:7192");
});







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

// Elsa için HttpClient
builder.Services.AddHttpClient("Elsa", client =>
{
    // 📌 Lokalde çalışırken bu portu kullan
    client.BaseAddress = new Uri("https://localhost:7192");

    // 📌 IIS’e deploy edince burayı güncelle:
    // client.BaseAddress = new Uri("https://intranet.firma.com/BeePM");
});
 


var app = builder.Build();

app.UseAuthentication(); // <-- ekledik
app.UseAuthorization();  
app.UseHttpsRedirection();
app.UseStaticFiles(); 
app.UseRouting();  
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 
app.MapControllers();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.MapRazorPages();
// ✅ Elsa 3 için doğru olan:
app.UseWorkflows(); 

app.Run();
