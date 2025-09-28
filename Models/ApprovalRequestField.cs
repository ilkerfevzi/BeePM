namespace BeePM.Models
{
    public class ApprovalRequestField
    {
        public int Id { get; set; }

        public int RequestId { get; set; }
        public ApprovalRequest? Request { get; set; }

        public int FieldDefinitionId { get; set; }
        public FieldDefinition? FieldDefinition { get; set; }

        public string? Value { get; set; }
    }
}
