using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Persistence.Core;

namespace Case.Energinet.Core.Models
{
    public interface ICachedRate : IEntity
    {
        double Rate { get; set; }
        CurrencyCodes ISOCode { get; set; }
        string Description { get; set; }
    }
}
