using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class TastedChampagneAdded : DomainEvent
    {
        public AggregateId ChampagneId { get; private set; }

        public TastedChampagneAdded(Guid id, AggregateId champagneId) : base(id)
        {
            ChampagneId = champagneId;
        }
    }
}