using System;
using SimpleSoft.Mediator;

namespace CM.Backend.Documents.Messages
{

    public class MessageEnvelope<TEvent> : Event
    {
        public TEvent Event { get; }

        public MessageEnvelope(TEvent @event, Guid aggregateId, DateTimeOffset createdOn, string createdBy = null)
        {
            Event = @event;
            Id = aggregateId;
            CreatedOn = createdOn;
            CreatedBy = createdBy;
        }
    }
}