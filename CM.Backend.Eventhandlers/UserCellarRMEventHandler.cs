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
    public class UserCellarRMEventHandler :
        IEventHandler<MessageEnvelope<TastingCreated>>,
        IEventHandler<MessageEnvelope<TastingDeleted>>
    {
        private readonly IUserCellarRepository userCellarRepository;


        public UserCellarRMEventHandler(IUserCellarRepository userCellarRepository)
        {
            this.userCellarRepository = userCellarRepository;
        }

        public async Task HandleAsync(MessageEnvelope<TastingCreated> evt, CancellationToken ct)
        {
            await userCellarRepository.Insert(new UserCellar
            {
                Id = Guid.NewGuid(),
                ChampagneId = evt.Event.ChampagneId.Value,
                UserId = evt.Event.AuthorId.Value,
                TastingId = evt.Id,
                PrimaryKey = new UserCellar.Key
                {
                    ChampagneId = evt.Event.ChampagneId.Value,
                    UserId = evt.Event.AuthorId.Value
                }
            });
        }

        public async Task HandleAsync(MessageEnvelope<TastingDeleted> evt, CancellationToken ct)
        {
            await userCellarRepository.DeleteTastedChampagne(new UserCellar.Key
            {
                ChampagneId = evt.Event.ChampagneId.Value,
                UserId = evt.Event.AuthorId.Value
            });
        }
    }
}