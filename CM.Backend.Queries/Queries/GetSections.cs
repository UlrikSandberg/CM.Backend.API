using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetSections : Query<IEnumerable<BrandPageSection>>
    {

		public Guid BrandId { get; private set; }

		public GetSections(Guid brandId)
        {
			BrandId = brandId;
        }
    }
}
