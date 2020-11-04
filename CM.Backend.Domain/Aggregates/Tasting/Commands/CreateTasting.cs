using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.Tasting.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting.Commands
{
    public class CreateTasting
    {
        public AggregateId Id { get; private set; }
        public string Review { get; private set; }
        public Rating Rating { get; private set; }
        public AggregateId AuthorId { get; private set; }
        public AggregateId ChampagneId { get; private set; }
        public AggregateId BrandId { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public bool IsPublic { get; private set; }
        public bool IsDeleted { get; private set; }

        public CreateTasting(AggregateId id, string review, Rating rating, AggregateId authorId, AggregateId champagneId, AggregateId brandId, DateTime timeStamp, bool isPublic)
        {
            if (id == null || rating == null || authorId == null || champagneId == null || brandId == null)
            {
                throw new ArgumentException(nameof(CreateTasting) + ": Some parameters are not allowed to be null");
            }
              
              
            Id = id;
            Review = review;
            Rating = rating;
            AuthorId = authorId;
            ChampagneId = champagneId;
            BrandId = brandId;
            TimeStamp = timeStamp;
            IsPublic = isPublic;
            IsDeleted = false;
        }
    }
}
