using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Wolf.Utility.Core.Logging;

namespace Case.Energinet.Frontend.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Startup StartupConfig;
        private readonly ILoggerManager logger;

        public App()
        {
            StartupConfig = new Startup();

            logger = StartupConfig.ServiceProvider.GetService<ILoggerManager>();
            logger.SetCaller(nameof(App));
            logger.LogInfo("Ready");
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = StartupConfig.ServiceProvider.GetService<MainWindow>();
            mainWindow.Init();
            mainWindow.Show();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            logger.LogInfo($"Closing Application. Goodbye.");
        }
    }
}
