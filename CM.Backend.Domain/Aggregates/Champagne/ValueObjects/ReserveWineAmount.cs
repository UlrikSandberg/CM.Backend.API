using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class ReserveWineAmount
    {
        public bool IsUnknown { get; private set; }
        public double Value { get; private set; }

        public ReserveWineAmount(double value)
        {
            if (value < 0.0 || value > 100.0)
            {
                throw new ArgumentException(nameof(value));
            }

            Value = value;
        }

        public ReserveWineAmount()
        {
            IsUnknown = true;
        }
    }
}