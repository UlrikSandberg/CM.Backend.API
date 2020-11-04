using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class DosageAmount
    {
        public double Value { get; private set; }
        public bool IsUnknown { get; private set; }

        public DosageAmount(double value)
        {
            if (value < 0)
            {
                throw new ArgumentException(nameof(value));
            }

            Value = value;
        }

        public DosageAmount()
        {
            IsUnknown = true;
        }
    }
}