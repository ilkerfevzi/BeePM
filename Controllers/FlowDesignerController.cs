using BeePM.Models;
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
                .OrderBy(f => f.RequestType)
                .ThenBy(f => f.StepNumber)
                .ToList();

            return View(flows);
        }

        [HttpPost]
        public IActionResult Create(ApprovalFlow model)
        {
            if (!IsAdmin()) return Unauthorized();
            if (!ModelState.IsValid) return View(model);

            _db.ApprovalFlows.Add(model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return Unauthorized();

            var flow = _db.ApprovalFlows.Find(id);
            if (flow == null) return NotFound();
            return View(flow);
        }

        [HttpPost]
        public IActionResult Edit(ApprovalFlow model)
        {
            if (!IsAdmin()) return Unauthorized();
            if (!ModelState.IsValid) return View(model);

            _db.ApprovalFlows.Update(model);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
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
    }
}
