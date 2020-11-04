using System.Collections.Generic;

namespace CM.Backend.Domain.SharedValueObjects
{
    public class NullValueObject<T> : ValueObject
    {
        public T Value { get; }

        public NullValueObject(T value)
        {
            Value = value;
        }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}