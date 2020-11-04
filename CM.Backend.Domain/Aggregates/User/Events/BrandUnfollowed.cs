using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class BrandUnfollowed : DomainEvent
    {
		public AggregateId BrandId { get; private set; }

		public BrandUnfollowed(Guid id, AggregateId brandId) : base(id)
        {
            BrandId = brandId;
		}
    }
}
