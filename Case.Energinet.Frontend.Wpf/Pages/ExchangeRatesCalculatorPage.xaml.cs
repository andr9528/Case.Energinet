using Case.Energinet.Core.Models;
using Case.Energinet.Core.Proxies;
using Case.Energinet.Persistence;
using Case.Energinet.Persistence.Models;
using Case.Energinet.Proxies;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
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
using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Logging;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Wolf.Utility.Core.Persistence.Exceptions;
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
        private ObservableCollection<ICachedRate> updatedRates = new();
        private List<string> isos = new();

        internal ObservableCollection<ICachedRate> UpdatedRates => updatedRates;

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

            DataContext = this;
        }

        public void Init() => Task.Run(async () => await SetupPage());

        private async Task SetupPage()
        {
            try
            {
                var cached = await handler.FindMultiple(new CachedRate());
                foreach (var rate in cached)
                {
                    updatedRates.Add(await ICachedRate.UpdateCachedRate(rate, handler, proxy, config.ExchangeRateMaxAge));
                }
            }
            catch (IncorrectEntityCountException<CachedRate> ice)
            {
                logger?.LogWarn($"No Cached rates was found in database. Perhabs none has been added yet? -> {ice.Message}");                
            }

            isos = Enum.GetNames(typeof(CurrencyCodes)).ToList().Where(x => x != $"NULL").ToList();

            Dispatcher.Invoke(() =>
            {
                ComboBoxISO.ItemsSource = isos;
                ComboBoxISO.SelectedIndex = 0;
                DataGridRates.ItemsSource = UpdatedRates;
                TextBlockInput.Text = "DKK";
                TextBlockOutput.Text = "???";
            });
        }

        private async Task UpdateConfigReference() 
        {
            if (!configNotNull) 
            {
                config = await MainWindow.RetrieveConfig(handler, logger);

                configNotNull = true;
            }
        }
        #region Calculator
        private void Button0_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button5_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button6_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button7_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button9_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonDecimal_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonEqual_Click(object sender, RoutedEventArgs e)
        {

        }   
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {

        }
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        private void ButtonCollect_Click(object sender, RoutedEventArgs e)
        {
            var iso = isos[ComboBoxISO.SelectedIndex];
            var succes = Enum.TryParse(iso, out CurrencyCodes isoCode);
            if (!succes) 
            {
                logger?.LogWarn($"Failed to parse '{iso}' into a valid {nameof(CurrencyCodes)}." +
                    $" Abandoning attempt to get Exchange rate..."); 
                return;
            }

            var check = updatedRates.Where(x => x.ISOCode == isoCode).ToList().Any();
            if (!check) Task.Run(async () => await GetAndAddCachedRate(new CachedRate() { ISOCode = isoCode }));
            else logger?.LogInfo($"Attempted to get Exchange rates for already existing one, which is unnecessary");
        }

        private async Task GetAndAddCachedRate(ICachedRate rate) 
        {
            var updated = await proxy.GetExchangeRate(rate);
            var check = updatedRates.Where(x => x.ISOCode == rate.ISOCode).ToList().Any();
            if (!check)
            {
                try
                {
                    var dbVersion = await handler.AddAndRetrieve(updated, false);
                    Dispatcher.Invoke(() => updatedRates.Add(dbVersion));                    
                }
                catch (Exception e)
                {                   
                    logger?.LogWarn($"An exception was thrown when attempting to {nameof(handler.AddAndRetrieve)} a {nameof(ICachedRate)}." +
                        $" Most likely there already existed an instance of the chosen ISO code ({rate.ISOCode}), " +
                        $"but could be something else, so see following message/stacktrace. -> {e}");
                }
            }
            else logger?.LogWarn($"Another call to {nameof(GetAndAddCachedRate)} has already added the requested Iso ({rate.ISOCode})." +
                $" No need for this one to add it too");
            

        }

        private void DataGridRates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var rate = updatedRates[DataGridRates.SelectedIndex];

            if (TextBlockOutput.Text.Length > 3) 
            {
                var split = TextBlockOutput.Text.Split(' ');
                split[1] = rate.ISOCode.ToString();
                var merge = split[0] + split[0];
                TextBlockOutput.Text = merge;
            }
            else TextBlockOutput.Text = rate.ISOCode.ToString();
        }
    }
}
