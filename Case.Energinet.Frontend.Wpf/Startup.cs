
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
using Case.Energinet.Core.Proxies;

namespace Case.Energinet.Frontend.Wpf
{
    public class Startup : ModularStartup
    {
        private const string CONNECTIONSTRINGNAME = "mainDb";
        public readonly string ConnectionString;
        public Startup() : base()
        {
            AddModule(new NLogStartupModule());
            AddModule(new WpfStartupModule<MainWindow>());

            ConnectionString = Configuration.GetConnectionString(CONNECTIONSTRINGNAME);
            AddModule(new EntityFrameworkStartupModule<EnerginetContext, EnerginetHandler>(
                options => { options.UseSqlite(ConnectionString); }));
            AddModule(new InlineStartupModule(setupServices: s => 
            {
                s.AddSingleton<INationalBankProxy, NationalBankProxy>();
            }));            

            SetupServices();
            SetupApplication();
        }
    }
}
