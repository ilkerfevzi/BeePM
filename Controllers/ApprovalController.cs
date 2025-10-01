using BeePM.Models;
using BeePM.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var req = _db.ApprovalRequests
                .Where(r => r.Id == id)
                .Select(r => new {
                    Request = r,
                    Template = r.FormTemplate!,
                    Elements = _db.FormElements.Where(e => e.FormTemplateId == r.FormTemplateId).OrderBy(e => e.OrderNo).ToList()
                })
                .FirstOrDefault();

            if (req == null) return NotFound();

            // JSON → Dictionary parse et
            var formData = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(req.Request.FormDataJson))
            {
                formData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(req.Request.FormDataJson) ?? new();
            }

            var vm = new RequestDetailsViewModel
            {
                Request = req.Request,
                Elements = req.Elements,
                FormData = formData
            };

            return View(vm);
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
            var templates = _db.FormTemplates.Include(t => t.Fields).ToList();
            ViewBag.FormTemplates = templates;
            return View(new ApprovalRequest());
        }
        [HttpPost("create")]
        public IActionResult CreateRequest(int formTemplateId, IFormCollection form)
        {
            var currentUser = _db.Users.First(u => u.Username == User.Identity!.Name);

            // Template alanlarını oku
            var template = _db.FormTemplates.Include(t => t.Fields)
                                            .FirstOrDefault(t => t.Id == formTemplateId);
            if (template == null) return NotFound();

            // Form verilerini JSON’a çevir
            var formData = new Dictionary<string, string>();
            foreach (var field in template.Fields)
            {
                var value = form[field.Label];
                formData[field.Label] = value!;
            }
            string formJson = System.Text.Json.JsonSerializer.Serialize(formData);

            var req = new ApprovalRequest
            {
                CreatedBy = currentUser.Id,
                CreatedAt = DateTime.Now,
                Status = "Pending",
                FormTemplateId = formTemplateId,
                FormDataJson = formJson
            };

            _db.ApprovalRequests.Add(req);
            _db.SaveChanges();

            TempData["msg"] = "Yeni onay süreci başlatıldı.";
            return RedirectToAction("MyRequests");
        }

        [HttpGet("fillform/{requestId}")]
        public IActionResult FillForm(int requestId)
        {
            var req = _db.ApprovalRequests
                .Where(r => r.Id == requestId)
                .Select(r => new {
                    Request = r,
                    Template = r.FormTemplate!,
                    Elements = _db.FormElements.Where(e => e.FormTemplateId == r.FormTemplateId).OrderBy(e => e.OrderNo).ToList()
                })
                .FirstOrDefault();

            if (req == null) return NotFound();

            var vm = new FillFormViewModel
            {
                RequestId = req.Request.Id,
                Title = req.Request.Title,
                Elements = req.Elements
            };

            return View(vm);
        }

        [HttpPost("fillform/{requestId}")]
        public IActionResult FillForm(int requestId, Dictionary<string, string> formValues)
        {
            var req = _db.ApprovalRequests.Find(requestId);
            if (req == null) return NotFound();

            req.FormDataJson = System.Text.Json.JsonSerializer.Serialize(formValues);
            _db.SaveChanges();

            TempData["msg"] = "Form başarıyla kaydedildi.";
            return RedirectToAction("MyRequests");
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
