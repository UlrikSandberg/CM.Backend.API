using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class ServingTemp
    {
        public bool IsUnknown { get; private set; }
        public double Value { get; private set; }

        public ServingTemp(double value)
        {
            Value = value;
        }

        public ServingTemp()
        {
            IsUnknown = true;
        }
    }
}