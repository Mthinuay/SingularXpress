using Microsoft.EntityFrameworkCore;
using SingularExpress.Models.Models;

namespace SingularExpress.Models
{
    public class ModelDbContext : DbContext
    {
        public ModelDbContext(DbContextOptions<ModelDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaxTable> TaxTables { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}