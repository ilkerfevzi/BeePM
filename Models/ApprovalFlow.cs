using System.ComponentModel.DataAnnotations;

namespace BeePM.Models
{
    public class ApprovalFlow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RequestType { get; set; } = "";

        [Required]
        public int StepNumber { get; set; }

        [Required]
        public string ApproverRole { get; set; } = "";

        [Required]
        public string ActionType { get; set; } = "";
    }
}
