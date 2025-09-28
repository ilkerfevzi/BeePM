namespace BeePM.Models
{
    public class FieldDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";  // Sistem içi
        public string Label { get; set; } = ""; // UI için
        public string FieldType { get; set; } = ""; // Textbox, Numeric, Combobox, Radio
        public string? Options { get; set; }     // JSON: ["Laptop","Telefon","Tablet"]

        public ICollection<ApprovalRequestField> RequestFields { get; set; } = new List<ApprovalRequestField>();
    }
}
