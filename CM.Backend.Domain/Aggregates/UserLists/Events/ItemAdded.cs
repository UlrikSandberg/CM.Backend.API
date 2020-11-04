using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class ItemAdded : DomainEvent
    {
        public AggregateId Item { get; }

        public ItemAdded(Guid id, AggregateId item) : base(id)
        {
            Item = item;
        }
    }
}