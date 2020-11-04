using System;
using System.Collections.Generic;

namespace CM.Backend.Domain.SharedValueObjects
{
    public class SingleValueObject<T> : ValueObject
    {
        public T Value { get; private set; }

        public SingleValueObject(T value)
        {
            if(value == null)
                throw new ArgumentException(nameof(value));
            
            Value = value;
        }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Value;
        }
    }
}