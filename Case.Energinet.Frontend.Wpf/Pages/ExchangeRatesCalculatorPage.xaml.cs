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
        private readonly MainWindow parent;
        private IConfig config;
        private ObservableCollection<ICachedRate> updatedRates = new();
        private List<string> isos = new();

        internal ObservableCollection<ICachedRate> UpdatedRates => updatedRates;

        public ExchangeRatesCalculatorPage(ILoggerManager logger, IHandler handler, INationalBankProxy proxy, IConfig config, bool configNotNull, MainWindow parent)
        {
            InitializeComponent();
            this.logger = logger;
            this.handler = handler;
            this.proxy = proxy;
            this.config = config;
            this.configNotNull = configNotNull;
            this.parent = parent;
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

                parent.KeyDown += Parent_KeyDown;
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
            if (TextBlockInput.Text.Count() > 3) WriteChar('0');
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('1');
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('2');
        }
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('3');
        }
        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('4');
        }
        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('5');
        }
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('6');
        }
        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('7');
        }
        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('8');
        }
        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            WriteChar('9');
        }
        private void ButtonDecimal_Click(object sender, RoutedEventArgs e)
        {
            if (!(TextBlockInput.Text.Count() > 3)) TextBlockInput.Text = "0, DKK";
            else if (!TextBlockInput.Text.Contains(',')) WriteChar(',');
        }
        private void WriteChar(char c) 
        {
            if (!(TextBlockInput.Text.Count() > 3)) TextBlockInput.Text = $"{c} DKK";
            else
            {
                var split = TextBlockInput.Text.Split(' ');

                var newNumber = split[0] + c;
                var merged = $"{newNumber} {split[1]}";
                TextBlockInput.Text = merged;
            }
        }
        private void ButtonEqual_Click(object sender, RoutedEventArgs e)
        {
            var rate = updatedRates[DataGridRates.SelectedIndex];

            Calculate(rate);
        }   
        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!(TextBlockInput.Text.Count() > 3)) return;
            if (TextBlockInput.Text.Count() == 5) TextBlockInput.Text = "DKK";
            else 
            {
                var split = TextBlockInput.Text.Split(' ');
                if (split[0] == "0,") TextBlockInput.Text = "DKK";
                else 
                {
                    var newNumber = split[0].Remove(split[0].Length - 1);
                    var merged = $"{newNumber} {split[1]}";
                    TextBlockInput.Text = merged;
                }

                
            }
        }
        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            TextBlockInput.Text = "DKK";
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

            if (TextBlockOutput.Text.Length > 3) Calculate(rate);
            else TextBlockOutput.Text = rate.ISOCode.ToString();
        }

        private void Calculate(ICachedRate rate) 
        {
            if (!(TextBlockInput.Text.Count() > 3) || TextBlockInput.Text == "0, DKK") 
            {
                logger?.LogWarn($"No valid number inside input field, when calling {nameof(Calculate)}. Aborting Calculate...");
                return;
            }

            var inputSplit = TextBlockInput.Text.Split(' ');
            var convert = double.TryParse(inputSplit[0], out var inputNumber);
            if (!convert) 
            {
                logger?.LogWarn($"Failed to parse '{inputSplit[0]}' into a valid double value. Aborting Calculate...");
                return;
            }

            var oneOfIso = 100 / rate.Rate;
            var result = inputNumber * oneOfIso;
            result = Math.Round(result, 4, MidpointRounding.ToEven);

            TextBlockOutput.Text = $"{result} {rate.ISOCode}";
        }

        private void Parent_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Decimal:
                case Key.OemComma:
                    ButtonDecimal_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D0:
                case Key.NumPad0:
                    Button0_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D1:
                case Key.NumPad1:
                    Button1_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D2:
                case Key.NumPad2:
                    Button2_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D3:
                case Key.NumPad3:
                    Button3_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D4:
                case Key.NumPad4:
                    Button4_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D5:
                case Key.NumPad5:
                    Button5_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D6:
                case Key.NumPad6:
                    Button6_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D7:
                case Key.NumPad7:
                    Button7_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D8:
                case Key.NumPad8:
                    Button8_Click(sender, new RoutedEventArgs());
                    break;
                case Key.D9:
                case Key.NumPad9:
                    Button9_Click(sender, new RoutedEventArgs());
                    break;
                case Key.Back:
                case Key.Delete:
                    ButtonDelete_Click(sender, new RoutedEventArgs());
                    break;
                case Key.Enter:
                    ButtonEqual_Click(sender, new RoutedEventArgs());
                    break;
                    

                default:
                    break;


            }
        }
    }
}
