using Case.Energinet.Core.Models;
using Case.Energinet.Core.Proxies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Case.Energinet.Persistence.Models
{
    public class CachedRate : ICachedRate
    {
        public double Rate { get; set; }
        public CurrencyCodes ISOCode { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public byte[] Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime PublishDate { get; set; }        
    }
}
