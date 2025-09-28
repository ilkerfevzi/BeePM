namespace BeePM.Models
{
    public class ApprovalViewModel
    {
        public List<ApprovalLog> Logs { get; set; } = new();
        public string LastDecision { get; set; } = string.Empty;
    }
}
