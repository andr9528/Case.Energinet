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

namespace Case.Energinet.Frontend.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        private readonly ILoggerManager logger;
        private readonly IHandler handler;

        public SettingsPage(ILoggerManager logger, IHandler handler)
        {
            InitializeComponent();
            this.logger = logger;
            this.handler = handler;
            logger.SetCaller(nameof(SettingsPage));

            logger.LogInfo("Ready");

            Title = "Settings";
        }
    }
}
