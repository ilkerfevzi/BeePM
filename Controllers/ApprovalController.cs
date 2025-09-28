using BeePM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace BeePM.Controllers
{
    [Route("approval")]
    public class ApprovalController : Controller
    {
        private readonly BeePMDbContext _db;
        private readonly IHttpClientFactory _httpFactory;

        public ApprovalController(BeePMDbContext db, IHttpClientFactory httpFactory)
        {
            _db = db;
            _httpFactory = httpFactory;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var logs = _db.ApprovalLogs
                          .OrderByDescending(x => x.Timestamp)
                          .Take(50)
                          .ToList();

            // Son karar
            string lastDecision = logs.FirstOrDefault()?.Message ?? "";

            var model = new ApprovalViewModel
            {
                Logs = logs,
                LastDecision = lastDecision
            };

            return View(model);
        }


        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var http = _httpFactory.CreateClient("Elsa");
            await http.PostAsync("/workflows/approval/start",
                new StringContent("", Encoding.UTF8, "text/plain"));

            _db.ApprovalLogs.Add(new ApprovalLog { Timestamp = DateTime.Now, Message = "Workflow başlatıldı", UserId = 1 });
            _db.SaveChanges();
            TempData["msg"] = "Workflow başlatıldı";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("step1")]
        public async Task<IActionResult> Step1(string decision)
        {
            var http = _httpFactory.CreateClient("Elsa");
            await http.PostAsync("/workflows/approval/step1",
                new StringContent(decision, Encoding.UTF8, "text/plain"));

            _db.ApprovalLogs.Add(new ApprovalLog { Timestamp = DateTime.Now, Message = $"Kullanıcı1 kararı: {decision}", UserId = 1 });
            _db.SaveChanges();
            TempData["msg"] = $"Step1 → {decision}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("step2")]
        public async Task<IActionResult> Step2(string decision)
        {
            var http = _httpFactory.CreateClient("Elsa");
            await http.PostAsync("/workflows/approval/step2",
                new StringContent(decision, Encoding.UTF8, "text/plain"));

            _db.ApprovalLogs.Add(new ApprovalLog { Timestamp = DateTime.Now, Message = $"Kullanıcı2 kararı: {decision}", UserId = 1 });
            _db.SaveChanges();
            TempData["msg"] = $"Step2 → {decision}";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("reset")]
        public IActionResult Reset()
        {
            _db.ApprovalLogs.RemoveRange(_db.ApprovalLogs);
            _db.SaveChanges();
            TempData["msg"] = "Log temizlendi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
