using System;
using SimpleSoft.Mediator;
using CM.Backend.Queries.Model;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandPage : Query<BrandPageExtendedSection>
    {
		public Guid BrandPageId { get; private set; }

		public GetBrandPage(Guid brandPageId)
        {
			BrandPageId = brandPageId;
        }
    }
}
