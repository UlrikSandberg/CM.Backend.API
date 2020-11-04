using System;
using SimpleSoft.Mediator;

namespace CM.Backend.Documents.Events
{
    public class DomainEvent : IDomainEvent
    {
        public DomainEvent(Guid id)
        {
            Id = id;
        }

        public new Guid Id { get; set; }
        public EventMetadata Metadata { get; set; }
    }
}