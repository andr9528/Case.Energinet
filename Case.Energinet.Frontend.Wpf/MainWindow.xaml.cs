using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wolf.Utility.Core.Logging;

using Wolf.Utility.Core.Wpf.Controls.Model;

using Wolf.Utility.Core.Wpf.Controls;
using Case.Energinet.Frontend.Wpf.Pages;
using Wolf.Utility.Core.Wpf.Core.Enums;
using Case.Energinet.Persistence;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Case.Energinet.Proxies;
using Case.Energinet.Persistence.Models;
using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Exceptions;
using Case.Energinet.Core.Models;
using Case.Energinet.Core.Proxies;
using Wolf.Utility.Core.Persistence.Exceptions;

namespace Case.Energinet.Frontend.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILoggerManager logger;
        private readonly IHandler handler;
        private NavigationPage navigation;
        private IConfig config = null;

        public MainWindow(ILoggerManager logger, IHandler handler)
        {
            InitializeComponent();
            this.logger = logger;
            this.handler = handler;
            logger.SetCaller(nameof(MainWindow));

            logger.LogInfo("Ready");            
        }

        public void Init() => Task.Run(async () => await CreateNavigationPage());

        public async Task CreateNavigationPage()
        {        
            config = await RetrieveConfig(handler, logger);

            Dispatcher.Invoke(() =>
            {
                var pages = new List<NavigationInfo>();
                var proxy = App.StartupConfig.ServiceProvider.GetService<INationalBankProxy>();

                //var test = proxy.GetExchangeRate(new CachedRate() { ISOCode = CurrencyCodes.EUR });

                var settingsLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
                var configNotNull = config != null ? true : false;

                var settingsPage = new SettingsPage(settingsLogger, handler, config, configNotNull);
                var settingsInfo = new NavigationInfo(settingsPage, desired: NavigationOrder.AbsoluteEnd);
                pages.Add(settingsInfo);

                var calculatorLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
                configNotNull = config != null ? true : false;

                var calculatorPage = new ExchangeRatesCalculatorPage(calculatorLogger, handler, proxy, config, configNotNull, this);
                calculatorPage.Init();
                var calculatorInfo = new NavigationInfo(calculatorPage);
                pages.Add(calculatorInfo);

                var navPageLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
                configNotNull = config != null ? true : false;
                if (configNotNull) logger?.LogWarn($"Config has not been retrived yet, when about to access for creating {nameof(NavigationPage)}");

                navigation = new NavigationPage(pages, navPageLogger,
                configNotNull ? config.NavigationLocation : NavigationLocation.Left,
                configNotNull ? !config.StartHidden : true)
                { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };

                MainFrame.Content = navigation;
            });


        }

        internal static async Task<Config> RetrieveConfig(IHandler handler, ILoggerManager logger) 
        {
            try
            {
                logger?.LogInfo($"Attempting to get Config to load from.");
                var config = (await handler.FindMultiple(new Config())).FirstOrDefault();
                logger?.LogInfo($"Succesfully retrieved Config to load from.");
                return config;
            }
            catch (IncorrectEntityCountException<Config> ice)
            {
                logger?.LogWarn($"Failed to find any config entity in database. Creating and using a new one instead. -> {ice.Message} {ice.StackTrace}");
                try
                {
                    return await handler.AddAndRetrieve(
                    new Config()
                    {
                        ExchangeRateMaxAge = new TimeSpan(1, 0, 0, 0),
                        NavigationLocation = NavigationLocation.Left,
                        StartHidden = true
                    }, false);
                }
                catch (Exception e)
                {
                    logger?.LogError($"Failed to Add and Retrieve new instance of Config. -> {e.Message} {e.StackTrace}");
                    throw;
                }
            }
        }
    }
}
