using Elsa.Identity.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeePM.Models
{
    public class ApprovalLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = string.Empty;

        // Foreign key
        public int UserId { get; set; }

        // Navigasyon
        public User? User { get; set; }
    }

}
