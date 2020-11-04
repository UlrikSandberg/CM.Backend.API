using System;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandPageSection : Query<BrandPageSection>
    {
		public Guid SectionId { get; private set; }

		public GetBrandPageSection(Guid sectionId)
        {
			SectionId = sectionId;
        }
    }
}
