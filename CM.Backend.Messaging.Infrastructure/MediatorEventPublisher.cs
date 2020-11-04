using System;
using System.Threading.Tasks;
using CM.Backend.Documents.Events;
using CM.Backend.Documents.Messages;
using CM.Backend.Messaging.Contracts;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Messaging.Infrastructure
{
    public class MediatorEventPublisher : IEventPublisher
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public MediatorEventPublisher(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : IDomainEvent
        {
            //In order for the eventhandler lookup to work, we need to publish with the concrete type of the event
            var eventType = @event.GetType();

            var msgType = typeof(MessageEnvelope<>).MakeGenericType(eventType);
            var msg = Activator.CreateInstance(msgType, @event, @event.Id, @event.Metadata?.EventDate ?? DateTimeOffset.UtcNow, null);
            
            var method = typeof(IMediator).GetMethod(nameof(IMediator.BroadcastAsync));
            var generic = method.MakeGenericMethod(msgType);

            _logger.Information("Publishing {EventName}: {@Event}", eventType.Name, @event);
            
            await (Task)generic.Invoke(_mediator, new []{msg, null});
        }
    }
}