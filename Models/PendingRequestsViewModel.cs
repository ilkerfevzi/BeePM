using System.Collections.Generic;

namespace BeePM.Models
{
    public class PendingRequestsViewModel
    {
        public List<ApprovalRequest> Requests { get; set; } = new();
    }
}
