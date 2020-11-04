using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections.Events
{
	public class SectionDeleted : DomainEvent
	{

		public AggregateId BrandId { get; private set; }
		public AggregateId BrandPageId { get; private set; }

		public SectionDeleted(Guid id, AggregateId brandId, AggregateId brandPageId) : base(id)
		{
			BrandId = brandId;
			BrandPageId = brandPageId;
		}
	}
}
