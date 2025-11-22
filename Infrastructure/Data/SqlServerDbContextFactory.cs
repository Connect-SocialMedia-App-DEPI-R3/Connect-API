using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public class SqlServerDbContextFactory : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlServerDbContext>();

            optionsBuilder.UseSqlServer("Server=db33180.public.databaseasp.net; Database=db33180; User Id=db33180; Password=2Zj#%6Wx7w+C; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;");

            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
}
