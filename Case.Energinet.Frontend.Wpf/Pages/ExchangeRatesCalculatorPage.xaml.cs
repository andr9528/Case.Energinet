using Case.Energinet.Core.Models;
using Case.Energinet.Core.Proxies;
using Case.Energinet.Persistence;
using Case.Energinet.Persistence.Models;
using Case.Energinet.Proxies;

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

using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Logging;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Wolf.Utility.Core.Wpf.Core.Enums;

namespace Case.Energinet.Frontend.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for ExchangeRatesCalculatorPage.xaml
    /// </summary>
    public partial class ExchangeRatesCalculatorPage : Page
    {
        private readonly ILoggerManager logger;
        private readonly IHandler handler;
        private readonly INationalBankProxy proxy;
        private bool configNotNull;
        private IConfig config;

        public ExchangeRatesCalculatorPage(ILoggerManager logger, IHandler handler, INationalBankProxy proxy, IConfig config, bool configNotNull)
        {
            InitializeComponent();
            this.logger = logger;
            this.handler = handler;
            this.proxy = proxy;
            this.config = config;
            this.configNotNull = configNotNull;
            logger.SetCaller(nameof(ExchangeRatesCalculatorPage));

            logger.LogInfo("Ready");

            Title = "Calculator";
        }

        private async Task UpdateConfigReference() 
        {
            if (!configNotNull) 
            {
                config = await MainWindow.RetrieveConfig(handler, logger);

                configNotNull = true;
            }
        }
    }
}
