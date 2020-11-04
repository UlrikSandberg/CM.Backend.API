using System;
using CM.Backend.Domain.Aggregates.Tasting.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting.Commands
{
    public class EditTasting
    {
        public string Review { get; private set; }
        public Rating Rating { get; private set; }

        public EditTasting(string review, Rating rating)
        {
            if (rating == null)
            {
                throw new ArgumentException(nameof(rating));
            }
            Review = review;
            Rating = rating;
        }
    }
}