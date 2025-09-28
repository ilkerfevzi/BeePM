using System.Collections.Generic;

namespace BeePM.Models
{
    public class RequestDetailsViewModel
    {
        public ApprovalRequest Request { get; set; } = null!;   // Sürecin ana kaydı
        public List<ApprovalLog> Logs { get; set; } = new();    // İlgili loglar
    }
}
