namespace BeePM.Models
{
    public class ApprovalRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Status { get; set; } = "Pending";
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<ApprovalRequestField> Fields { get; set; } = new List<ApprovalRequestField>();
    }
}
