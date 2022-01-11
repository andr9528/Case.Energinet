using Case.Energinet.Core.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Wpf.Core.Enums;

namespace Case.Energinet.Persistence.Models
{
    public class Config : IConfig
    {
        public TimeSpan ExchangeRateMaxAge { get; set; }
        public bool StartHidden { get; set; }
        public NavigationLocation NavigationLocation { get; set; }
        public int Id { get; set; }
        public byte[] Version { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
