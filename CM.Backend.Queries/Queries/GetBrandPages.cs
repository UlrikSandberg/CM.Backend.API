using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandPages : Query<IEnumerable<BrandPage>>
    {
		public Guid BrandId { get; private set; }

		public GetBrandPages(Guid brandId)
        {
			BrandId = brandId;
        }
    }
}
