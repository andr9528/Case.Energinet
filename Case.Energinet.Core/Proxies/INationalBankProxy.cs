using Case.Energinet.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Case.Energinet.Core.Proxies
{
    public interface INationalBankProxy
    {
        Task<ICachedRate> GetExchangeRate(ICachedRate cache);
    }
}
