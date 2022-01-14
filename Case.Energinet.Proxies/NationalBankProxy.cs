

using Case.Energinet.Core.Models;
using Case.Energinet.Core.Proxies;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Logging;

namespace Case.Energinet.Proxies
{
    // Followed: https://docs.microsoft.com/en-us/windows/communitytoolkit/parsers/rssparser
    public class NationalBankProxy : INationalBankProxy
    {
        private const string NationalBank = "https://www.nationalbanken.dk/_vti_bin/DN/DataService.svc/CurrencyRateRSS?lang=da&iso=";
        private readonly ILoggerManager logger;

        public NationalBankProxy(ILoggerManager logger)
        {
            this.logger = logger;

            logger.SetCaller(nameof(NationalBankProxy));

            logger.LogInfo("Ready");
        }

        public async Task<ICachedRate> GetExchangeRate(ICachedRate cache) 
        {
            if (cache.ISOCode == CurrencyCodes.NULL) throw new ArgumentNullException(nameof(cache.ISOCode));

            var uri = NationalBank + cache.ISOCode.ToString();
            SyndicationFeed feed = null;

            try
            {
                using (var reader = XmlReader.Create(uri))
                {
                    feed = SyndicationFeed.Load(reader);                    
                }
            }
            catch (Exception e)
            {
                logger?.LogError($"An Exception was thrown: {e.Message} -> {e.StackTrace}");
                throw;
            }

            if (feed != null) 
            {
//#if DEBUG
//                var count = 1;
//                foreach (var syndicationItem in feed.Items)
//                {
//                    logger?.LogDebug($"Item nr. {count}:");
//                    logger?.LogDebug($"Title -> {syndicationItem.Title.Text}");
//                    logger?.LogDebug($"Content -> {syndicationItem.Content.Type}");
//                    logger?.LogDebug($"PublishDate -> {syndicationItem.PublishDate}");
//                    logger?.LogDebug($"LastUpdatedTime -> {syndicationItem.LastUpdatedTime}");
//                    count++;
//                }
//#endif

                var item = feed.Items.FirstOrDefault();

                if (item != null) 
                {
                    double rate = default;

                    var parse = double.TryParse(item.Title.Text.Split('(')[1].TrimEnd(')'), out rate);
                    if (parse) logger?.LogDebug($"Succesfully parsed '{item.Title.Text}' into a double '{rate}'");
                    else logger?.LogDebug($"Failed to parse '{item.Title.Text}' into a double");

                    cache.Rate = rate;
                    cache.PublishDate = item.PublishDate.ToLocalTime().DateTime;
                    cache.Description = $"100 {cache.ISOCode} koster {cache.Rate} DKK.";
                }
            }

            return cache;
        }
    }

}
