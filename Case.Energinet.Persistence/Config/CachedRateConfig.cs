using Case.Energinet.Persistence.Models;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Case.Energinet.Persistence.Config
{
    public class CachedRateConfig : EntityConfig<CachedRate>
    {
        public CachedRateConfig()
        {
        }

        public override void Configure(EntityTypeBuilder<CachedRate> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x=>x.ISOCode).IsUnique();
        }
    }
}
