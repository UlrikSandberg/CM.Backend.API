using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Tasting.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting.Events
{
    public class TastingEdited : DomainEvent
    {
        public AggregateId ChampagneId { get; private set; }
        public string Review { get; private set; }
        public Rating Rating { get; private set; }

        public TastingEdited(Guid id, AggregateId champagneId, string review, Rating rating) : base(id)
        {
            ChampagneId = champagneId;
            Review = review;
            Rating = rating;
        }
    }
}