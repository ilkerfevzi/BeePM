using System;
using System.Collections.Generic;

namespace BeePM.Models
{
    public class ApprovalRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public int CreatedBy { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string RequestedItem { get; set; } = string.Empty;

        // 🔹 Yeni alan: Hangi şablonla dolduruldu
        public int FormTemplateId { get; set; }
        public FormTemplate? FormTemplate { get; set; }

        // 🔹 Formdaki cevapları JSON olarak saklayacağız
        public string? FormDataJson { get; set; }



        // 🔗 İlişki
        //public User? CreatedUser { get; set; }
        public ICollection<ApprovalRequestField> Fields { get; set; } = new List<ApprovalRequestField>();
    }
}
