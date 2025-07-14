using Microsoft.EntityFrameworkCore;
using SingularExpress.Models.Models;

namespace SingularExpress.Models
{
    public class ModelDbContext : DbContext
    {
        public ModelDbContext(DbContextOptions<ModelDbContext> options) : base(options) { }

        public DbSet<TaxTable> TaxTables { get; set; }
        public DbSet<TaxTableEntry> TaxTableEntries { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxTableEntry>()
                .Property(t => t.AnnualEquivalent)
                .HasPrecision(18, 2);

            modelBuilder.Entity<TaxTableEntry>()
                .Property(t => t.TaxUnder65)
                .HasPrecision(18, 2);
        }
    }
}