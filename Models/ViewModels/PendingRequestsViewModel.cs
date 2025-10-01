using System.Collections.Generic;

namespace BeePM.Models.ViewModels
{
    public class PendingRequestsViewModel
    {
        public List<ApprovalRequest> Requests { get; set; } = new();
    }
}
