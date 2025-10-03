using System.ComponentModel.DataAnnotations;

namespace BeePM.Models
{
    public class ApprovalFlow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string? FlowJson { get; set; }
    }
}
