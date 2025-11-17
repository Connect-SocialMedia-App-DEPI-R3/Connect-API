using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        private IConfiguration _config;

        AppDbContextFactory(IConfiguration configuration)
        {
            _config = configuration;
        }
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseNpgsql(_config.GetConnectionString("PostgreSqlConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
