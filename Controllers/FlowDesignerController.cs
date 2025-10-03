using BeePM.Models;
using Elsa.Workflows.Activities.Flowchart.Activities;
using Microsoft.AspNetCore.Mvc;

namespace BeePM.Controllers
{
    public class FlowDesignerController : Controller
    {
        private readonly BeePMDbContext _db;

        public FlowDesignerController(BeePMDbContext db)
        {
            _db = db;
        }
        private bool IsAdmin()
        {
            var currentUser = _db.Users.FirstOrDefault(u => u.Username == User.Identity!.Name);
            return currentUser != null && currentUser.Role == "Admin";
        }

        public IActionResult Index()
        {
            if (!IsAdmin())
                return Unauthorized(); // 🚫 erişim reddi

            var flows = _db.ApprovalFlows
                .OrderBy(f => f.Name)
                .ThenBy(f => f.Description)
                .ToList();

            return View(flows);
        }
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin()) return Unauthorized();
            return View(new ApprovalFlow());
        }
        [HttpPost]
        public IActionResult Create(ApprovalFlow model)
        {
            if (!IsAdmin()) return Unauthorized();
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.Now;
            _db.ApprovalFlows.Add(model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var flow = _db.ApprovalFlows.FirstOrDefault(f => f.Id == id);
            if (flow == null) return NotFound();

            ViewBag.FlowJson = string.IsNullOrEmpty(flow.FlowJson) ? "{}" : flow.FlowJson;
            return View(flow);
        }

        [HttpPost]
        public IActionResult Save(int id, string flowJson)
        {
            if (!IsAdmin()) return Unauthorized();

            var flow = _db.ApprovalFlows.FirstOrDefault(f => f.Id == id);
            if (flow == null) return NotFound();

            flow.FlowJson = flowJson;
            // flow.CreatedAt = DateTime.Now;  // ❌ Artık güncellemiyoruz
            _db.SaveChanges();

            return Json(new { success = true });
        }

        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var flow = _db.ApprovalFlows.Find(id);
            if (flow == null) return NotFound();

            _db.ApprovalFlows.Remove(flow);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // GET: /FlowDesigner/Users?term=adm
        [HttpGet]
        public IActionResult Users(string term)
        {
            var q = (term ?? "").Trim();

            var users = _db.Users
                .Where(u => q == "" ||
                            u.FullName.Contains(q) ||
                            u.Username.Contains(q))
                .OrderBy(u => u.FullName)
                .Take(20)
                .Select(u => new
                {
                    // Select2 "id/text" sözleşmesi
                    id = u.Username,                           // XML’de camunda:assignee olarak saklanacak
                    text = u.FullName + " (" + u.Username + ")"
                })
                .ToList();

            return Json(users);
        }











    }
}
