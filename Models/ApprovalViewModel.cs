namespace BeePM.Models
{
    public class ApprovalViewModel
    {
        public List<ApprovalLog> Logs { get; set; } = new ();
        public string Status { get; set; } = "Idle";   // Idle, WaitingStep1, WaitingStep2, Rejected, Done
        public bool ShowStep1 => Status == "WaitingStep1";
        public bool ShowStep2 => Status == "WaitingStep2"; 
    }
}
