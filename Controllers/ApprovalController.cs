using BeePM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BeePM.Controllers;

[Route("approval")]
public class ApprovalController : Controller
{
    private readonly BeePMDbContext _db;
    private readonly HttpClient _http; 

    private readonly IHttpClientFactory _httpFactory;

    public ApprovalController(IHttpClientFactory httpFactory)
    {
        _httpFactory = httpFactory;
    }

    [HttpPost("step1")]
    public async Task<IActionResult> Step1(string decision)
    {
        var currentUser = _db.Users.FirstOrDefault(u => u.Username == User.Identity!.Name);

        _db.ApprovalLogs.Add(new ApprovalLog
        {
            Timestamp = DateTime.Now,
            Message = $"Kullanıcı1 → {decision}",
            UserId = currentUser?.Id ?? 0 // Kullanıcı bulunamazsa 0 yaz, istersen "system" kullanıcı ekle
        });
        _db.SaveChanges();

        var http = _httpFactory.CreateClient();
        var res = await http.PostAsync("/workflows/approval/step1",
            new StringContent(decision, Encoding.UTF8, "text/plain"));

        TempData["msg"] = $"Step1 → {(int)res.StatusCode} {res.ReasonPhrase}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("step2")]
    public async Task<IActionResult> Step2(string decision)
    {
        var currentUser = _db.Users.FirstOrDefault(u => u.Username == User.Identity!.Name);
        if (currentUser == null)
        {
            currentUser = new User
            {
                Username = User.Identity!.Name ?? "system",
                FullName = "Otomatik Kullanıcı"
            };
            _db.Users.Add(currentUser);
            _db.SaveChanges();
        }

        _db.ApprovalLogs.Add(new ApprovalLog
        {
            Timestamp = DateTime.Now,
            Message = $"Kullanıcı → {decision}",
            UserId = currentUser.Id
        });
        _db.SaveChanges();

        var http = _httpFactory.CreateClient("Elsa");
        var res = await http.PostAsync("/workflows/approval/step2",
            new StringContent(decision, Encoding.UTF8, "text/plain"));

        TempData["msg"] = $"Kullanıcı2 → {decision}";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("approval/send")]
    public async Task<IActionResult> Send(string karar)
    {
        using var client = new HttpClient();
        var url = "https://localhost:5001/workflows/approval-callback"; // Elsa HttpEndpoint adresi
        var content = new StringContent(karar, System.Text.Encoding.UTF8, "text/plain");

        var response = await client.PostAsync(url, content);
        var respText = await response.Content.ReadAsStringAsync();

        ViewBag.Response = respText;
        return View("Index");
    }
}
