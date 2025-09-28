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
        // Benim süreçlerim
        [HttpGet("myrequests")]
        public IActionResult MyRequests()
        {
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);
            var myReqs = _db.ApprovalRequests
                .Where(r => r.CreatedBy == currentUser.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var model = new MyRequestsViewModel { Requests = myReqs };
            return View(model);
        }

        // Onayımdaki süreçler
        [HttpGet("pendingrequests")]
        public IActionResult PendingRequests()
        {
            var requests = _db.ApprovalRequests
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.CreatedAt)
                .ToList();

            var model = new PendingRequestsViewModel { Requests = requests };
            return View(model);
        }

        [HttpGet("history")]
        public IActionResult History()
        {
            var logs = _db.ApprovalLogs
                .OrderByDescending(x => x.Timestamp)
                .Take(200) // en fazla 200 kayıt
                .ToList();

            return View(logs);
        }

        [HttpGet("details/{id}")]
        public IActionResult Details(int id)
        {
            var req = _db.ApprovalRequests.Find(id);
            if (req == null) return NotFound();

            var logs = _db.ApprovalLogs.Where(l => l.UserId == req.CreatedBy || l.Message.Contains($"Request {id}"))
                                       .OrderByDescending(l => l.Timestamp)
                                       .ToList();

            var model = new RequestDetailsViewModel
            {
                Request = req,
                Logs = logs
            };

            return View(model);
        }

        [HttpGet("logs")]
        public IActionResult Logs()
        {
            var logs = _db.ApprovalLogs
                .OrderByDescending(x => x.Timestamp)
                .Take(200)
                .Select(l => new
                {
                    l.Id,
                    l.Timestamp,
                    l.Message,
                    UserFullName = l.User != null ? l.User.FullName : "—",
                    UserRole = l.User != null ? l.User.Role : "—"
                })
                .ToList()
                .Select(l => new ApprovalLog
                {
                    Id = l.Id,
                    Timestamp = l.Timestamp,
                    Message = l.Message,
                    User = new User { FullName = l.UserFullName, Role = l.UserRole }
                })
                .ToList();

            return View(logs);
        }




        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            var http = _httpFactory.CreateClient("Elsa");
            await http.PostAsync("/workflows/approval/start",
                new StringContent("", Encoding.UTF8, "text/plain"));
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);

            _db.ApprovalLogs.Add(new ApprovalLog { Timestamp = DateTime.Now, Message = "Workflow başlatıldı", UserId = currentUser.Id });
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
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);

            _db.ApprovalLogs.Add(new ApprovalLog { Timestamp = DateTime.Now, Message = $"Kullanıcı1 kararı: {decision}", UserId = currentUser.Id });
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
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);

            _db.ApprovalLogs.Add(new ApprovalLog { Timestamp = DateTime.Now, Message = $"Kullanıcı2 kararı: {decision}", UserId = currentUser.Id });
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
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new ApprovalRequest());
        }
        [HttpPost("create")]
        public IActionResult CreateRequest(ApprovalRequest request)
        {
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);
            request.CreatedBy = currentUser.Id;
            request.Status = "Pending";
            request.CreatedAt = DateTime.Now; 

            _db.ApprovalRequests.Add(request);
            _db.SaveChanges();

            TempData["msg"] = "Yeni onay süreci başlatıldı.";
            return RedirectToAction("PendingRequests");
        }


        [HttpPost]
        public IActionResult Decide(int requestId, string decision)
        {
            var req = _db.ApprovalRequests.Find(requestId);
            if (req == null) return NotFound();

            req.Status = decision == "Onay" ? "Approved" : "Rejected";
            _db.SaveChanges();
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);
             
            _db.ApprovalLogs.Add(new ApprovalLog
            {
                Timestamp = DateTime.Now,
                Message = $"User kararı: {decision}",
                UserId = currentUser.Id
            });
            _db.SaveChanges();

            return RedirectToAction("PendingRequests");
        }
        // Geçmiş süreçler
        public IActionResult CompletedRequests()
        {
            var requests = _db.ApprovalRequests
                .Where(r => r.Status == "Approved" || r.Status == "Rejected")
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            var model = new CompletedRequestsViewModel { Requests = requests };
            return View(model);
        }


    }
}
