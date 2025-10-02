using BeePM.Models;
using Microsoft.EntityFrameworkCore;

namespace BeePM.Models
{
    public class BeePMDbContext : DbContext
    {
        public BeePMDbContext(DbContextOptions<BeePMDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ApprovalLog> ApprovalLogs => Set<ApprovalLog>();
        public DbSet<ApprovalRequest> ApprovalRequests => Set<ApprovalRequest>(); 
        public DbSet<FieldDefinition> FieldDefinitions { get; set; }
        public DbSet<ApprovalRequestField> ApprovalRequestFields { get; set; }
        public DbSet<ApprovalFlow> ApprovalFlows { get; set; }
        //public DbSet<FormTemplate> FormTemplates { get; set; }
        public DbSet<FormElement> FormElements { get; set; }
        //public DbSet<FormField> FormFields { get; set; }
        // 🔽 EKLE:
        public DbSet<FormTemplate> FormTemplates => Set<FormTemplate>();
        public DbSet<FormField> FormFields => Set<FormField>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FormTemplate>(e =>
            {
                e.ToTable("FormTemplates");
                e.HasKey(t => t.Id);
                e.Property(t => t.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<FormField>(e =>
            {
                e.ToTable("FormFields");
                e.HasKey(f => f.Id);
                e.Property(f => f.Label).IsRequired().HasMaxLength(200);
                e.Property(f => f.FieldType).IsRequired().HasMaxLength(50);

                // 🔗 İLİŞKİYİ AÇIKLA
                e.HasOne(f => f.FormTemplate)
                 .WithMany(t => t.Fields)
                 .HasForeignKey(f => f.FormTemplateId)
                 .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
