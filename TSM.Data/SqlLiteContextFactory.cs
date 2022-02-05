using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TSM.Data
{
    public class SqlLiteContextFactory : IDesignTimeDbContextFactory<SqlLiteDbContext>
    {
        public SqlLiteDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlLiteDbContext>();
            optionsBuilder.UseSqlite("Data Source=TSM.db");

            return new SqlLiteDbContext(optionsBuilder.Options);
        }
    }
}