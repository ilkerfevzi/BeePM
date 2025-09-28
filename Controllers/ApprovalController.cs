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

            var last = logs.FirstOrDefault()?.Message ?? "";
            string status = last switch
            {
                "" => "Idle",
                "Workflow başlatıldı" => "WaitingStep1",
                "Kullanıcı1 kararı: Onay" => "WaitingStep2",
                "Kullanıcı1 kararı: Red" => "Rejected",
                _ when last.StartsWith("Kullanıcı2 kararı:") => "Done",
                _ => "Idle"
            };

            var model = new ApprovalViewModel { Logs = logs, Status = status };
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
        [HttpPost]
        public IActionResult CreateRequest(ApprovalRequest request)
        {
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);
            request.CreatedBy = currentUser.Id;
            request.Status = "Pending";
            _db.ApprovalRequests.Add(request);
            _db.SaveChanges();

            TempData["msg"] = "Yeni onay süreci başlatıldı.";
            return RedirectToAction("Index");
        }
        public IActionResult PendingRequests()
        {
            var requests = _db.ApprovalRequests
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.CreatedAt)
                .ToList();

            return View(requests);
        }
        [HttpPost]
        public IActionResult Decide(int requestId, string decision)
        {
            var req = _db.ApprovalRequests.Find(requestId);
            if (req == null) return NotFound();

            req.Status = decision == "Onay" ? "Approved" : "Rejected";
            _db.SaveChanges();

            _db.ApprovalLogs.Add(new ApprovalLog
            {
                Timestamp = DateTime.Now,
                Message = $"User2 kararı: {decision}",
                UserId = 2 // şimdilik hardcoded
            });
            _db.SaveChanges();

            return RedirectToAction("PendingRequests");
        }
        public IActionResult MyRequests()
{
    var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);
    var myReqs = _db.ApprovalRequests
        .Where(r => r.CreatedBy == currentUser.Id)
        .ToList();
    return View(myReqs);
}

    }
}
