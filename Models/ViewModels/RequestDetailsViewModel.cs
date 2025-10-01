using System.Collections.Generic;

namespace BeePM.Models.ViewModels
{
    public class RequestDetailsViewModel
    {
        public ApprovalRequest Request { get; set; } = new();
        public List<FormElement> Elements { get; set; } = new();
        public Dictionary<string, string> FormData { get; set; } = new();
    }
}
