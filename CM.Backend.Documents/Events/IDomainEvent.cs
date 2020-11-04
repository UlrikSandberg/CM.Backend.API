using System;
using SimpleSoft.Mediator;

namespace CM.Backend.Documents.Events
{
    public interface IDomainEvent
    {
        Guid Id { get; }
        EventMetadata Metadata { get; set; }
    }
}