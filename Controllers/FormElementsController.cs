using BeePM.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeePM.Controllers
{
    public class FormElementsController : Controller
    {
        private readonly BeePMDbContext _db;

        public FormElementsController(BeePMDbContext db)
        {
            _db = db;
        }

        // Element listesi
        public IActionResult Index(int templateId)
        {
            var template = _db.FormTemplates.FirstOrDefault(f => f.Id == templateId);
            if (template == null) return NotFound();

            var elements = _db.FormElements
                              .Where(e => e.FormTemplateId == templateId)
                              .OrderBy(e => e.OrderNo)
                              .ToList();

            ViewBag.TemplateName = template.Name;
            ViewBag.TemplateId = templateId;
            return View(elements);
        }

        // Yeni element
        public IActionResult Create(int templateId)
        {
            ViewBag.TemplateId = templateId;
            return View(new FormElement { FormTemplateId = templateId });
        }

        [HttpPost]
        public IActionResult Create(FormElement element)
        {
            if (ModelState.IsValid)
            {
                _db.FormElements.Add(element);
                _db.SaveChanges();
                return RedirectToAction("Index", new { templateId = element.FormTemplateId });
            }
            return View(element);
        }

        // Düzenleme
        public IActionResult Edit(int id)
        {
            var element = _db.FormElements.Find(id);
            if (element == null) return NotFound();
            return View(element);
        }

        [HttpPost]
        public IActionResult Edit(FormElement element)
        {
            if (ModelState.IsValid)
            {
                _db.FormElements.Update(element);
                _db.SaveChanges();
                return RedirectToAction("Index", new { templateId = element.FormTemplateId });
            }
            return View(element);
        }

        // Silme
        public IActionResult Delete(int id)
        {
            var element = _db.FormElements.Find(id);
            if (element != null)
            {
                int tid = element.FormTemplateId;
                _db.FormElements.Remove(element);
                _db.SaveChanges();
                return RedirectToAction("Index", new { templateId = tid });
            }
            return NotFound();
        }
    }
}
