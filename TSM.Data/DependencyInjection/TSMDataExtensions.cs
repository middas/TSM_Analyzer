using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TSM.Core.LocalStorage;

namespace TSM.Data.DependencyInjection
{
    public static class TSMDataExtensions
    {
        public static ServiceCollection ConfigureTSMData(this ServiceCollection service, string dbPath)
        {
            string? dirPath = Path.GetDirectoryName(dbPath);

            if (!string.IsNullOrEmpty(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            service.AddDbContext<SqlLiteDbContext>(options =>
            {
                options.UseSqlite($"Data Source = {dbPath}");
            });
            service.AddTransient<IDataStore, DataStore>();

            return service;
        }
    }
}