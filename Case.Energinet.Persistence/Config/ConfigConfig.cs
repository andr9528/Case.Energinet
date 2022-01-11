using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Case.Energinet.Persistence.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Case.Energinet.Persistence.Config
{
    public class ConfigConfig : EntityConfig<Models.Config>
    {
        public ConfigConfig()
        {
        }
        public override void Configure(EntityTypeBuilder<Models.Config> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ExchangeRateMaxAge).HasConversion(new TimeSpanToTicksConverter());
        }
    }
}
