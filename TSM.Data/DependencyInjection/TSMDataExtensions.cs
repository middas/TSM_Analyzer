using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
