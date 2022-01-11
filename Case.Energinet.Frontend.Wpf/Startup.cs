
using Case.Energinet.Persistence;

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Wolf.Utility.Core.Logging;
using Wolf.Utility.Core.Persistence.EntityFramework;
using Wolf.Utility.Core.Startup;
using Wolf.Utility.Core.Wpf.Startup;
using Wolf.Utility.Core.Startup.Modules;
using Microsoft.Extensions.DependencyInjection;
using Case.Energinet.Proxies;

namespace Case.Energinet.Frontend.Wpf
{
    public class Startup : ModularStartup
    {
        private const string CONNECTIONSTRINGNAME = "mainDb";
        public Startup() : base()
        {
            AddModule(new NLogStartupModule());
            AddModule(new WpfStartupModule<MainWindow>());
            AddModule(new EntityFrameworkStartupModule<EnerginetContext, EnerginetHandler>(
                options => { options.UseSqlite(Configuration.GetConnectionString(CONNECTIONSTRINGNAME)); }));
            AddModule(new InlineStartupModule(setupServices: s => 
            {
                s.AddSingleton<NationalBankProxy>();
            }));            

            SetupServices();
            SetupApplication();
        }
    }
}
