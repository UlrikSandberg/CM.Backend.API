using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Events
{
	public class ChampagnePublishStatusChanged : DomainEvent
    {
		public bool IsPublished { get; private set; }
	    public AggregateId BrandId { get; private set; }

	    public ChampagnePublishStatusChanged(Guid id, bool isPublished, AggregateId brandId) : base(id)
	    {
		    IsPublished = isPublished;
		    BrandId = brandId;
	    }
    }
}
