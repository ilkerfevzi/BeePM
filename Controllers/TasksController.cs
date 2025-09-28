using BeePM.Models;
using BeePM.Services;
using Elsa.Workflows.Options;
using Elsa.Workflows.Runtime;
using Microsoft.AspNetCore.Mvc; 
using System.Net.Http;
using System.Text; 

namespace BeePM.Controllers;

[Route("tasks")]
public class TasksController : Controller
{
    private readonly BeePMDbContext _db;
    private readonly HttpClient _http;
    public TasksController(BeePMDbContext db, IHttpClientFactory httpFactory)
    {
        _db = db;
        _http = httpFactory.CreateClient("Elsa");
    }

    [HttpGet("")]
    public IActionResult Index()
    { 
        var logs = _db.ApprovalLogs
            .OrderByDescending(x => x.Timestamp)
            .Take(50) // Son 50 kaydı gösterelim
            .ToList();

        // Eğer TempData mesajı varsa log’a ekleyelim
        if (TempData["msg"] is string msg)
        {
            logs.Add(new ApprovalLog
            {
                Timestamp = DateTime.Now,
                Message = msg
            });
        }

        return View(logs);
    }

    // ▶ Workflow’u BAŞLAT (HttpEndpoint: /workflows/approval/start)
    [HttpPost("start")]
    public async Task<IActionResult> Start()
    {
        var res = await _http.PostAsync("/workflows/approval/start",
            new StringContent("", Encoding.UTF8, "text/plain"));

        var msg = $"▶ Start → {(int)res.StatusCode} {res.ReasonPhrase}";
        SaveLog(msg);

        TempData["msg"] = msg;
        return RedirectToAction(nameof(Index));
    }
    // ✅ Onay
    [HttpPost("approve")]
    public async Task<IActionResult> Approve()
    {
        var res = await _http.PostAsync("/workflows/approval/callback",
            new StringContent("Onayla", Encoding.UTF8, "text/plain"));

        var msg = $"✅ Onay → {(int)res.StatusCode} {res.ReasonPhrase}";
        SaveLog(msg);

        TempData["msg"] = msg;
        return RedirectToAction(nameof(Index));
    }

    // ❌ Red
    [HttpPost("reject")]
    public async Task<IActionResult> Reject()
    {
        var res = await _http.PostAsync("/workflows/approval/callback",
            new StringContent("Reddet", Encoding.UTF8, "text/plain"));

        var msg = $"❌ Red → {(int)res.StatusCode} {res.ReasonPhrase}";
        SaveLog(msg);

        TempData["msg"] = msg;
        return RedirectToAction(nameof(Index));
    }

    // 🔹 DB’ye log ekleme yardımcı metodu
    private void SaveLog(string message)
    {
        _db.ApprovalLogs.Add(new ApprovalLog
        {
            Timestamp = DateTime.Now,
            Message = message
        });
        _db.SaveChanges();
    }
}
