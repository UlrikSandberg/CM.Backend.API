using System;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandImage : Query<BrandImage>
    {
		public Guid BrandId { get; private set; }
		public Guid Id { get; private set; }

		public GetBrandImage(Guid id, Guid brandId)
        {
            Id = id;
			BrandId = brandId;
		}
    }
}
