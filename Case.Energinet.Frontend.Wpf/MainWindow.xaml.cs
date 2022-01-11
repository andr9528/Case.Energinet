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

namespace Case.Energinet.Frontend.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILoggerManager logger;
        private readonly IHandler handler;
        private readonly NavigationPage navigation;

        public MainWindow(ILoggerManager logger, IHandler handler)
        {
            InitializeComponent();
            this.logger = logger;
            this.handler = handler;
            logger.SetCaller(nameof(MainWindow));

            logger.LogInfo("Ready");

            navigation = CreateNavigationPage();
            MainFrame.Content = navigation;
        }

        public NavigationPage CreateNavigationPage()
        {
            var pages = new List<NavigationInfo>();
            var proxy = App.StartupConfig.ServiceProvider.GetService<NationalBankProxy>();

            var test = proxy.GetExchangeRate(new CachedRate() { ISOCode = CurrencyCodes.EUR });

            var settingsLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
            var settingsPage = new SettingsPage(settingsLogger, handler);
            var settingsInfo = new NavigationInfo(settingsPage, desired: NavigationOrder.AbsoluteEnd);
            pages.Add(settingsInfo);

            var viewerLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
            var viewerPage = new ExchangeRatesViewerPage(viewerLogger, handler, proxy);
            var viewerInfo = new NavigationInfo(viewerPage);
            pages.Add(viewerInfo);

            var calculatorLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
            var calculatorPage = new ExchangeRatesCalculatorPage(calculatorLogger, handler, proxy);
            var calculatorInfo = new NavigationInfo(calculatorPage);
            pages.Add(calculatorInfo);

            var navPageLogger = App.StartupConfig.ServiceProvider.GetService<ILoggerManager>();
            var navPage = new NavigationPage(pages, navPageLogger, NavigationLocation.Left) { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
            return navPage;
        }
    }
}
