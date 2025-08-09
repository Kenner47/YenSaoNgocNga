using Microsoft.EntityFrameworkCore;

namespace PaymentManagement_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // DbSets for Payment Management Entities
        // Example: public DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure entities, relationships, and constraints here
            // Example: modelBuilder.Entity<Payment>().HasKey(p => p.PaymentId);
        }
    }
}
