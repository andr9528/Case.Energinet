using Case.Energinet.Core.Models;
using Case.Energinet.Persistence;

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
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Wolf.Utility.Core.Wpf.Core.Enums;

namespace Case.Energinet.Frontend.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly ILoggerManager logger;
        private readonly IHandler handler;
        private IConfig config;
        private bool configNotNull;
        private int saveCount = 0;
        private bool saving = false;

        public SettingsPage(ILoggerManager logger, IHandler handler, IConfig config, bool configNotNull)
        {
            InitializeComponent();
            this.logger = logger;
            this.handler = handler;
            this.config = config;
            this.configNotNull = configNotNull;
            logger.SetCaller(nameof(SettingsPage));

            logger.LogInfo("Ready");

            Title = "Settings";

            Task.Run(async() => await LoadFromConfig());
        }

        private async Task UpdateConfigReference()
        {
            if (!configNotNull)
            {
                config = await MainWindow.RetrieveConfig(handler, logger);

                configNotNull = true;
            }
        }

        private async Task LoadFromConfig()
        {
            await UpdateConfigReference();

            Dispatcher.Invoke(() => 
            { 
                CheckBoxStartHidden.IsChecked = config.StartHidden;
                TimeSpanMaxAgePicker.Value = config.ExchangeRateMaxAge;

                var locations = Enum.GetNames(typeof(NavigationLocation)).ToList();
                locations.RemoveAt(4);
                ComboBoxBurgerLocation.ItemsSource = locations;
                ComboBoxBurgerLocation.SelectedIndex = (int)config.NavigationLocation;  
            });
        } 

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (saving == true) return;

            config.StartHidden = (bool)CheckBoxStartHidden.IsChecked;
            config.ExchangeRateMaxAge = (TimeSpan)TimeSpanMaxAgePicker.Value;
            config.NavigationLocation = (NavigationLocation)ComboBoxBurgerLocation.SelectedIndex;

            SaveChanges();

            saveCount++;
            TextBlockSave.Text = $"Changes Saved ({saveCount} times)";
        }

        private async Task SaveChanges()
        {
            try
            {
                saving = true;

                config = await handler.UpdateAndRetrieve(config);
            }
            catch (Exception e)
            {
                logger?.LogError($"Exception was thrown while updating the Config entity. -> {e.Message} {e.StackTrace}");
                throw;
            }
            finally 
            {
                saving = false;
            }

            
        }

        private void CheckBoxStartHidden_Checked(object sender, RoutedEventArgs e)
        {
            saveCount = 0;
        }

        private void ComboBoxBurgerLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            saveCount = 0;
        }

        private void TimeSpanMaxAgePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            saveCount = 0;
        }
    }
}
