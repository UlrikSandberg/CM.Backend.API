using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Events
{
	public class SectionRemoved : DomainEvent
	{
		public AggregateId BrandId { get; private set; }
		public AggregateId SectionId { get; private set; }

		public SectionRemoved(Guid id, AggregateId brandId, AggregateId sectionId) : base(id)
		{
			SectionId = sectionId;
			BrandId = brandId;
		}
	}
}
