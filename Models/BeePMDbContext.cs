using Microsoft.EntityFrameworkCore;

namespace BeePM.Models
{
    public class BeePMDbContext : DbContext
    {
        public BeePMDbContext(DbContextOptions<BeePMDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ApprovalLog> ApprovalLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApprovalLog>()
                .HasOne(a => a.User)
                .WithMany(u => u.ApprovalLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
