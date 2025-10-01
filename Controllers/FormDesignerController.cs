using BeePM.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BeePM.Controllers
{
    public class FormDesignerController : Controller
    {
        private readonly BeePMDbContext _db;

        public FormDesignerController(BeePMDbContext db)
        {
            _db = db;
        }

        // Şablon listesi
        public IActionResult Index()
        {
            var templates = _db.FormTemplates.ToList();
            return View(templates);
        }

        // Yeni şablon
        [HttpGet]
        public IActionResult Create()
        {
            return View(new FormTemplate());
        }

        [HttpPost]
        public IActionResult Create(FormTemplate template)
        {
            if (ModelState.IsValid)
            {
                _db.FormTemplates.Add(template);
                _db.SaveChanges();
                return RedirectToAction(nameof(Edit), new { id = template.Id });
            }
            return View(template);
        }

        // Şablon düzenleme (alan ekleme/designer ekranı)
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var template = _db.FormTemplates
                .Where(t => t.Id == id)
                .FirstOrDefault();

            if (template == null) return NotFound();

            var fields = _db.FormFields.Where(f => f.FormTemplateId == id).OrderBy(f => f.Order).ToList();

            var vm = new FormDesignerViewModel
            {
                Template = template,
                Fields = fields
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult AddField(int templateId, FormField field)
        {
            field.FormTemplateId = templateId;
            _db.FormFields.Add(field);
            _db.SaveChanges();

            return RedirectToAction("Edit", new { id = templateId });
        }
    }

    public class FormDesignerViewModel
    {
        public FormTemplate Template { get; set; }
        public List<FormField> Fields { get; set; } = new();
    }
}
