using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandChampagnes : Query<IEnumerable<Champagne>>
    {
		public Guid BrandId { get; private set; }

		public GetBrandChampagnes(Guid brandId)
        {
            BrandId = brandId;
		}
    }
}
