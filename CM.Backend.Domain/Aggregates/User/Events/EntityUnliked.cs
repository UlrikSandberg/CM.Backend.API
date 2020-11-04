using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class EntityUnliked : DomainEvent
    {
        public AggregateId UnlikeToContextId { get; private set; }

        public EntityUnliked(Guid id, AggregateId unlikeToContextId) : base(id)
        {
            UnlikeToContextId = unlikeToContextId;
        }
    }
}