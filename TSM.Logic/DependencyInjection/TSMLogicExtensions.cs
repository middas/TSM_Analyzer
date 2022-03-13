using Microsoft.Extensions.DependencyInjection;
using TSM.Data.DependencyInjection;

namespace TSM.Logic.DependencyInjection
{
    public static class TSMLogicExtensions
    {
        public static ServiceCollection ConfigureTSMLogic(this ServiceCollection service, string dbPath)
        {
            service.ConfigureTSMData(dbPath);

            return service;
        }
    }
}