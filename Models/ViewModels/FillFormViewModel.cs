namespace BeePM.Models.ViewModels
{
    public class FillFormViewModel
    {
        public int RequestId { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<FormElement> Elements { get; set; } = new();
    }
}
