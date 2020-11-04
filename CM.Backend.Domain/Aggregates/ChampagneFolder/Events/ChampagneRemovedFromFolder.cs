using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Events
{
    public class ChampagneRemovedFromFolder : DomainEvent
    {
        public AggregateId ChampagneId { get; private set; }

        public ChampagneRemovedFromFolder(Guid id, AggregateId champagneId) : base(id)
        {
            ChampagneId = champagneId;
        }
    }
}