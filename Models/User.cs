namespace BeePM.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }

        // Navigasyon
        public ICollection<ApprovalLog> ApprovalLogs { get; set; } = new List<ApprovalLog>();
    }
}
