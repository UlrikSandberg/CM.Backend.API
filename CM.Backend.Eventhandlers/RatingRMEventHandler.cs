using System;
using System.Threading;
using System.Threading.Tasks;
using Baseline;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.Tasting.Events;
using CM.Backend.Persistence.Model.RatingModel;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    
    public class RatingRMEventHandler :
        IEventHandler<MessageEnvelope<TastingCreated>>,
        IEventHandler<MessageEnvelope<TastingEdited>>,
        IEventHandler<MessageEnvelope<TastingDeleted>>
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly IChampagneRepository _champagneRepository;

        private const string ChampagneRatingType = "Champagne";
        
        public RatingRMEventHandler(IRatingRepository ratingRepository, IChampagneRepository champagneRepository)
        {
            _ratingRepository = ratingRepository;
            _champagneRepository = champagneRepository;
        }
        
        public async Task HandleAsync(MessageEnvelope<TastingCreated> evt, CancellationToken ct)
        {
            //The key should be consist of a combined key from both the Id of the thing rated as well as the UserId.
            await _ratingRepository.Insert(new RatingModel
            {
                Id = evt.Event.ChampagneId.Value,
                Key = new RatingModel.PrimaryKey { EntityId = evt.Event.ChampagneId.Value, UserId = evt.Event.AuthorId.Value },
                ContextId = evt.Id,
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow,
                Rating = evt.Event.Rating.Value,
                Type = ChampagneRatingType,
                UserId = evt.Event.AuthorId.Value
            });

            await UpdateChampagneRatings(evt.Event.ChampagneId.Value);
        }

        public async Task HandleAsync(MessageEnvelope<TastingEdited> evt, CancellationToken ct)
        {
            //Update the tasting
            await _ratingRepository.UpdateRatingModelByContextId(evt.Id, evt.Event.Rating.Value);

            await UpdateChampagneRatings(evt.Event.ChampagneId.Value);
        }

        public async Task HandleAsync(MessageEnvelope<TastingDeleted> evt, CancellationToken ct)
        {
            //Delete the respective rating
            await _ratingRepository.DeleteRatingModel(new RatingModel.PrimaryKey
                {UserId = evt.Event.AuthorId.Value, EntityId = evt.Event.ChampagneId.Value});

            await UpdateChampagneRatings(evt.Event.ChampagneId.Value);
        }

        private async Task UpdateChampagneRatings(Guid champagneId)
        {
            var result = await _ratingRepository.GetEntityAverageRatingAndCount(champagneId);
            
            //Update the respective champagnes averageRating and number of ratings.
            await _champagneRepository.UpdateChampagneAverageRatingAndRatingCount(champagneId,
                result.AverageRating, result.RatingCount, result.RatingValue);
        }
    }
}