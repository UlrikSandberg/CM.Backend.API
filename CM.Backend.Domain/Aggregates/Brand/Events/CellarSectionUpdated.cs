using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
    public class CellarSectionUpdated : DomainEvent
    {
        public AggregateId SectionId { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public List<AggregateId> Champagnes { get; private set; }

        public CellarSectionUpdated(Guid id, AggregateId sectionId, string title, string body, List<AggregateId> champagnes) : base(id)
        {
            SectionId = sectionId;
            Title = title;
            Body = body;
            Champagnes = champagnes;
        }
    }
}