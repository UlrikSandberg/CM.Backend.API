using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandImages : Query<IEnumerable<BrandImage>>
    {
		public Guid BrandId { get; private set; }

		public GetBrandImages(Guid brandId)
        {
			BrandId = brandId;
        }
    }
}
