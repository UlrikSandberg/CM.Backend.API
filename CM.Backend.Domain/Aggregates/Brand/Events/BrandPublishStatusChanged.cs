using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
	public class BrandPublishStatusChanged : DomainEvent
	{
		public AggregateId BrandId { get; private set; }
		public bool PublishStatus { get; private set; }

		public BrandPublishStatusChanged(Guid id, AggregateId brandId, bool publishStatus) : base(id)
		{
			BrandId = brandId;
			PublishStatus = publishStatus;
		}
	}
}
