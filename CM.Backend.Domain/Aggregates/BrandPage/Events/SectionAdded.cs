using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Events
{
	public class SectionAdded : DomainEvent
    {
		public AggregateId BrandId { get; private set; }
		public AggregateId SectionId { get; private set; }

		public SectionAdded(Guid id, AggregateId brandId, AggregateId sectionId) : base(id)
        {
			BrandId = brandId;
			SectionId = sectionId;
        }

    }
}
