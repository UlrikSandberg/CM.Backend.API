using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting.Events
{
    public class TastingDeleted : DomainEvent
    {
        public AggregateId AuthorId { get; private set; }
        public AggregateId ChampagneId { get; private set; }
        public bool IsDeleted { get; private set; }

        public TastingDeleted(Guid id, AggregateId authorId, AggregateId champagneId, bool isDeleted) : base(id)
        {
            AuthorId = authorId;
            ChampagneId = champagneId;
            IsDeleted = isDeleted;
        }
    }
}