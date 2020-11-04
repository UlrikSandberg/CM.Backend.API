using System;
using SimpleSoft.Mediator;
using CM.Backend.Queries.Model;

namespace CM.Backend.Queries.Queries
{
	public class GetCellar : Query<Cellar>
    {
		public Guid BrandId { get; private set; }

		public GetCellar(Guid brandId)
        {
			BrandId = brandId;
        }
    }
}
