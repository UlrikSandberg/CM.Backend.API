using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class GrapeAmount
    {
        public bool IsUnknown { get; private set; }
        public double Value { get; private set; }

        public GrapeAmount(double value)
        {
            if (value < 0.0 || value > 100.0)
            {
                throw new ArgumentException(nameof(GrapeAmount) + ": GrapeAmount can't be higher");
            }

            Value = value;
        }

        public GrapeAmount()
        {
            IsUnknown = true;
        }
    }
}