using Case.Energinet.Persistence.Config;
using Case.Energinet.Persistence.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.EntityFramework;

namespace Case.Energinet.Persistence
{
    public class EnerginetContext : BaseContext
    {
        public DbSet<Models.Config> Configs { get; set; }
        public DbSet<CachedRate> CachedRates { get; set; }

        public EnerginetContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CachedRateConfig());
            modelBuilder.ApplyConfiguration(new ConfigConfig());
        }
    }
}
