using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class AgeingPotential
    {
        public bool IsUnknown { get; private set; }
        public double Value { get; private set; }

        public AgeingPotential(double value)
        {
            if (value < 0.0)
            {
                throw new ArgumentException(nameof(value));
            }

            Value = value;
        }
        
        public AgeingPotential()
        {
            IsUnknown = true;
        }
    }
}