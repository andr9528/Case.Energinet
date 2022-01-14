using Case.Energinet.Core.Models;
using Case.Energinet.Persistence.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Extensions.Money.Enums;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework;

namespace Case.Energinet.Persistence
{
    public class EnerginetHandler : BaseHandler<EnerginetContext>
    {
        public EnerginetHandler(EnerginetContext context) : base(context)
        {
        }

        protected async override Task<IQueryable<T>> AbstractFind<T>(T predicate)
        {
            IQueryable<T> query = null;

            switch (predicate)
            {
                case IConfig c:
                    query = (IQueryable<T>)await BuildQuery(c, Context.Configs.AsQueryable());
                    break;
                case ICachedRate c:
                    query = (IQueryable<T>)await BuildQuery(c, Context.CachedRates.AsQueryable());
                    break;
                default:
                    break;
            }

            return query ?? throw new NullReferenceException($"Query was Null in {nameof(EnerginetHandler)}");
        }

        private async Task<IQueryable<CachedRate>> BuildQuery(ICachedRate c, IQueryable<CachedRate> query)
        {
            if (c.ISOCode != default)
                query = query.Where(x=>x.ISOCode == c.ISOCode);

            if (c.Rate != default)
                query = query.Where(x=>x.Rate == c.Rate);

            if (c.Description != default)
                query = query.Where(x=>x.Description.Contains(c.Description));

            return query;
        }

        private async Task<IQueryable<Models.Config>> BuildQuery(IConfig c, IQueryable<Models.Config> query)
        {
            query = query.Where(x => x.Id == 1);

            return query;
        }
    }
}
