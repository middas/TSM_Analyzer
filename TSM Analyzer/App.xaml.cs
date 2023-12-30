using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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
#if RELEASE
        private static readonly string dataStorePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TSM Analyzer", "TSM.db");
#else
        private static readonly string dataStorePath = "TSM.db";
#endif

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
            services.ConfigureTSMLogic(dataStorePath);
            services.AddSingleton<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}