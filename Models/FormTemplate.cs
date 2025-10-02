using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeePM.Models
{
    public class FormTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        //public int CreatedBy { get; set; }
        //public User? CreatedByUser { get; set; }

        public ICollection<FormField> Fields { get; set; } = new List<FormField>();
    }
    public class FormField
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public string Label { get; set; } = "";
        public string FieldType { get; set; } = ""; // Textbox, Combobox...
        public string? Options { get; set; }
        public bool IsRequired { get; set; }
        public int OrderNo { get; set; } = 0;             // Form içindeki sıralama

        [ForeignKey(nameof(FormTemplateId))]
        public FormTemplate? FormTemplate { get; set; }
        //public FormTemplate? Template { get; set; }
        //public FormTemplate FormTemplate { get; set; }  // Navigation
    }
    public class FormElement
    {
        public int Id { get; set; }
        public int FormTemplateId { get; set; }
        public FormTemplate? FormTemplate { get; set; }

        public string Label { get; set; } = string.Empty;
        public string ElementType { get; set; } = string.Empty;  // "TextBox", "RadioButton", "ComboBox"
        public string? Options { get; set; }   // JSON format: ["Evet","Hayır"] gibi
        public bool IsRequired { get; set; }
        public int OrderNo { get; set; }
    }
}
