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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);

            //// ✅ Seed Users
            //modelBuilder.Entity<User>().HasData(
            //    new User { Id = 1, Username = "employee1", FullName = "User 1 Çalışan", Role = "Employee" },
            //    new User { Id = 2, Username = "manager1", FullName = "User 2 Müdür", Role = "Manager" },
            //    new User { Id = 3, Username = "board1", FullName = "User 3 Yönetim Kurulu", Role = "Board" },
            //    new User { Id = 4, Username = "admin", FullName = "Admin Kullanıcı", Role = "Admin" }
            //);
        }
    }
}
