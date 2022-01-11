using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Wpf.Core.Models;

namespace Case.Energinet.Core.Models
{
    public interface IConfig : INavigationPageConfig, IEntity
    {
        TimeSpan ExchangeRateMaxAge { get; set; }
    }
}
