using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Events
{
	public class BrandPagePublished : DomainEvent
	{
		public AggregateId BrandId { get; private set; }
		public AggregateId BrandPageId { get; private set; }
		public bool Publish { get; private set; }

		public BrandPagePublished(Guid id, AggregateId brandId, AggregateId brandPageId, bool publish) : base(id)
		{
			BrandId = brandId;
			BrandPageId = brandPageId;
			Publish = publish;
		}
	}
}
