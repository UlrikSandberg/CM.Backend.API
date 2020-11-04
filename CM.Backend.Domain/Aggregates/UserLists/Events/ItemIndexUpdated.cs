using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class ItemIndexUpdated : DomainEvent
    {
        public AggregateId ItemId { get; }
        public int Index { get; }

        public ItemIndexUpdated(Guid id, AggregateId itemId, int index) : base(id)
        {
            ItemId = itemId;
            Index = index;
        }
    }
}