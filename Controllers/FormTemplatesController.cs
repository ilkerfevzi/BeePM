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

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index()
        {
            var templates = _db.FormTemplates.Include(f => f.Fields).ToList();
            return View(templates);
        }

        public IActionResult Create()
        {
            return View(new FormTemplate());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FormTemplate template)
        {
            if (ModelState.IsValid)
            {
                _db.FormTemplates.Add(template);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(template);
        }
        // GET: /FormTemplates/Edit/5 → Designer
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var template = _db.FormTemplates.Include(t => t.Fields).FirstOrDefault(t => t.Id == id);
            if (template == null) return NotFound();

            // Alanları da yükleyelim
            var fields = _db.FormFields.Where(f => f.FormTemplateId == id).ToList();
            ViewBag.Fields = fields;

            return View(template);
        }

        // POST: /FormTemplates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FormTemplate template)
        {
            if (ModelState.IsValid)
            {
                _db.FormTemplates.Update(template);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(template);
        }

        // Alan ekleme (Designer içinden çağrılacak)
        [HttpPost]
        public IActionResult AddField(int templateId, string label, string fieldType, bool isRequired, string? options)
        {
            var field = new FormField
            {
                FormTemplateId = templateId,
                Label = label,
                FieldType = fieldType,
                IsRequired = isRequired,
                Options = options
            };

            _db.FormFields.Add(field);
            _db.SaveChanges();
            return RedirectToAction("Edit", new { id = templateId });
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
