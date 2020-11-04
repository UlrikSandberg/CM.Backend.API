using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Tasting.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting.Events
{
    public class TastingCreated : DomainEvent
    {
        public string Review { get; private set; }
        public Rating Rating { get; private set; }
        public AggregateId AuthorId { get; private set; }
        public AggregateId ChampagneId { get; private set; }
        public AggregateId BrandId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public bool IsPublic { get; private set; }
        public bool IsDeleted { get; private set; }

        public TastingCreated(Guid id, string review, Rating rating, AggregateId authorId, AggregateId champagneId, AggregateId brandId, bool isDeleted, DateTime timeStamp, bool isPublic) : base(id)
        {
            Review = review;
            Rating = rating;
            AuthorId = authorId;
            ChampagneId = champagneId;
            BrandId = brandId;
            TimeStamp = timeStamp;
            IsPublic = isPublic;
            IsDeleted = isDeleted;
        }
    }
}
