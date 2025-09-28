using System;
using System.Collections.Generic;

namespace BeePM.Models
{
    public class ApprovalRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string RequestedItem { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string Status { get; set; } = "Pending";

        // 🔗 İlişki
        public User? CreatedUser { get; set; }
        public ICollection<ApprovalRequestField> Fields { get; set; } = new List<ApprovalRequestField>();
    }
}
