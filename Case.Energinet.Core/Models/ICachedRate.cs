using Case.Energinet.Core.Proxies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Case.Energinet.Core.Models
{
    public interface ICachedRate : IEntity
    {
        double Rate { get; set; }
        CurrencyCodes ISOCode { get; set; }
        string Description { get; set; }
        DateTime PublishDate { get; set; }

        public static async Task<ICachedRate> UpdateCachedRate(ICachedRate cached, IHandler handler, INationalBankProxy proxy, TimeSpan maxAge)
        {
            if (DateTime.Now - cached.UpdatedDate > maxAge)
            {
                await proxy.GetExchangeRate(cached);

                return await handler.UpdateAndRetrieve(cached);
            }
            return cached;
        }
    }
}
