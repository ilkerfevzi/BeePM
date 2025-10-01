using BeePM.Models;
using Microsoft.EntityFrameworkCore;

namespace BeePM.Models
{
    public class BeePMDbContext : DbContext
    {
        public BeePMDbContext(DbContextOptions<BeePMDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ApprovalLog> ApprovalLogs { get; set; } = null!;
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<FieldDefinition> FieldDefinitions { get; set; }
        public DbSet<ApprovalRequestField> ApprovalRequestFields { get; set; }
        public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
        public DbSet<FormTemplate> FormTemplates { get; set; }
        public DbSet<FormElement> FormElements { get; set; }
        public DbSet<FormField> FormFields { get; set; }
    }
}
