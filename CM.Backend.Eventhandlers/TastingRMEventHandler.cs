using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.Tasting.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class TastingRMEventHandler :
        IEventHandler<MessageEnvelope<TastingCreated>>,
        IEventHandler<MessageEnvelope<TastingEdited>>,
        IEventHandler<MessageEnvelope<TastingDeleted>>
    {
        private readonly ITastingRepository tastingRepository;

        public TastingRMEventHandler(ITastingRepository tastingRepository)
        {
            this.tastingRepository = tastingRepository;
        }

        public async Task HandleAsync(MessageEnvelope<TastingCreated> evt, CancellationToken ct)
        {
            await tastingRepository.Insert(new Tasting
            {
                Id = evt.Id,
                Review = evt.Event.Review,
                Rating = evt.Event.Rating.Value,
                AuthorId = evt.Event.AuthorId.Value,
                ChampagneId = evt.Event.ChampagneId.Value,
                BrandId = evt.Event.BrandId.Value,
                TastedOnDate = evt.Event.TimeStamp
            });
        }

        public async Task HandleAsync(MessageEnvelope<TastingEdited> evt, CancellationToken ct)
        {
            await tastingRepository.UpdateTasting(evt.Id, evt.Event.Review, evt.Event.Rating.Value);
        }

        public async Task HandleAsync(MessageEnvelope<TastingDeleted> evt, CancellationToken ct)
        {
            await tastingRepository.Delete(evt.Id);
        }
    }
}