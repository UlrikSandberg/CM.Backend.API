using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Tasting.ValueObjects
{
    public class Rating : SingleValueObject<double>
    {
        public Rating(double value) : base(value)
        {
            if (value < 0.0 || value > 5.0)
            {
                throw new ArgumentException(nameof(value));
            }
        }
    }
}