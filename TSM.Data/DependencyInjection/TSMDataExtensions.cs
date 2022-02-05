using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TSM.Core.LocalStorage;

namespace TSM.Data.DependencyInjection
{
    public static class TSMDataExtensions
    {
        public static ServiceCollection ConfigureTSMData(this ServiceCollection service)
        {
            service.AddDbContext<SqlLiteDbContext>(options =>
            {
                options.UseSqlite("Data Source = TSM.db");
            });
            service.AddTransient<IDataStore, DataStore>();

            return service;
        }
    }
}