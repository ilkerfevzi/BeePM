using BeePM.Models;

namespace BeePM.Services
{
    public class ApprovalLogService
    {
        private static readonly List<ApprovalLog> _logs = new();

        public void Add(string msg)
        {
            _logs.Add(new ApprovalLog { Message = msg });
        }

        public List<ApprovalLog> GetAll()
        {
            return _logs.OrderByDescending(x => x.Timestamp).ToList();
        }
    }
}
