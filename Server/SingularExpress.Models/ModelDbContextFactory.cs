using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SingularExpress.Models
{
    public class ModelDbContextFactory : IDesignTimeDbContextFactory<ModelDbContext>
    {
        public ModelDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read appsettings.json (adjust path if needed)
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // Usually the root where you run dotnet ef
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ModelDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new ModelDbContext(optionsBuilder.Options);
        }
    }
}
