using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TSM.Core.LocalStorage;
using TSM.Logic.DependencyInjection;

namespace TSM_Analyzer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider serviceProvider;

        public App()
        {
            ServiceCollection services = new();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();

            ConfigureDataStore(serviceProvider.GetRequiredService<IDataStore>());
        }

        private void ConfigureDataStore(IDataStore dataStore)
        {
            dataStore.Configure();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services.ConfigureTSMLogic();
        }
    }
}