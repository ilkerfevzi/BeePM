using System.Collections.Generic;

namespace BeePM.Models.ViewModels
{
    public class CompletedRequestsViewModel
    {
        public List<ApprovalRequest> Requests { get; set; } = new();
    }
}
