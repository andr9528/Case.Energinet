using Case.Energinet.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Case.Energinet.Frontend.Wpf
{
    /// <summary>
    ///  Used by EFC during Migration to create the context.
    /// </summary>
    public class EnerginetContextFactory : IDesignTimeDbContextFactory<EnerginetContext>
    {
        public EnerginetContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EnerginetContext>();
            optionsBuilder.UseSqlite(App.StartupConfig.ConnectionString);

            return new EnerginetContext(optionsBuilder.Options);
        }
    }
}
