using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use the same provider you configure at runtime
            optionsBuilder.UseSqlite("Data Source=connect.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
