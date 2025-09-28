namespace BeePM.Models
{
    public class FieldDefinition
    {
        public int Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string FieldType { get; set; } = "Text"; // Text, Numeric, Dropdown, Radio vs.
        public string? Options { get; set; } // JSON veya CSV olarak saklanabilir
    }
}
