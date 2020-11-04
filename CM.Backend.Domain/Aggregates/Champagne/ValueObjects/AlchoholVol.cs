using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class AlchoholVol
    {
        public bool IsUnknown { get; private set; }
        public double Value { get; private set; }

        public AlchoholVol(double value)
        {
            if (value < 0 || value > 100)
            {
                throw new ArgumentException(nameof(AlchoholVol));
            }

            Value = value;
        }
        
        public AlchoholVol()
        {
            IsUnknown = true;
        }
    }
}