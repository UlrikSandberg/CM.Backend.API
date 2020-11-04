using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class ItemRemoved : DomainEvent
    {
        public AggregateId Item { get; }

        public ItemRemoved(Guid id, AggregateId item) : base(id)
        {
            Item = item;
        }
    }
}