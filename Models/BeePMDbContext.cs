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
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; } = null!;
        public DbSet<FieldDefinition> FieldDefinitions { get; set; } = null!;
        public DbSet<ApprovalRequestField> ApprovalRequestFields { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Seed örneği
            modelBuilder.Entity<FieldDefinition>().HasData(
                new FieldDefinition
                {
                    Id = 1,
                    Name = "Reason",
                    Label = "Talep Nedeni",
                    FieldType = "Textbox"
                },
                new FieldDefinition
                {
                    Id = 2,
                    Name = "Item",
                    Label = "Talep Edilen Ürün",
                    FieldType = "Combobox",
                    Options = "[\"Laptop\",\"Telefon\",\"Tablet\"]"
                },
                new FieldDefinition
                {
                    Id = 3,
                    Name = "Quantity",
                    Label = "Adet",
                    FieldType = "Numeric"
                }
            );
        }
    }
}
