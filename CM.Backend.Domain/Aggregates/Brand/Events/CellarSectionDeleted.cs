using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
    public class CellarSectionDeleted : DomainEvent
    {
        public AggregateId SectionId { get; private set; }

        public CellarSectionDeleted(Guid id, AggregateId sectionId) : base(id)
        {
            SectionId = sectionId;
        }
    }
}