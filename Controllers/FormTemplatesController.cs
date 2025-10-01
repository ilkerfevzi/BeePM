using BeePM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeePM.Controllers
{
    public class FormTemplatesController : Controller
    {
        private readonly BeePMDbContext _db;

        public FormTemplatesController(BeePMDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var templates = _db.FormTemplates.Include(f => f.Elements).ToList();
            return View(templates);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(FormTemplate template)
        {
            template.CreatedBy = 1; // TODO: oturum açan kullanıcıdan al
            template.CreatedAt = DateTime.Now;
            _db.FormTemplates.Add(template);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var template = _db.FormTemplates.Include(f => f.Elements).FirstOrDefault(f => f.Id == id);
            if (template == null) return NotFound();
            return View(template);
        }

        [HttpPost]
        public IActionResult Edit(FormTemplate template)
        {
            _db.FormTemplates.Update(template);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var template = _db.FormTemplates.Find(id);
            if (template != null)
            {
                _db.FormTemplates.Remove(template);
                _db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
