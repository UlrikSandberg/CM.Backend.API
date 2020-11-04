using System;
using System.Collections.Generic;

namespace CM.Backend.Domain.SharedValueObjects
{
    public class AggregateId : SingleValueObject<Guid>, IEqualityComparer<AggregateId>
    {
        public AggregateId(Guid value) : base(value)
        {
            if(value == null || value.Equals(Guid.Empty))
                throw new ArgumentException(nameof(value) + " : AggregateId may not be null nor Guid.Empty");
        }

        public bool Equals(AggregateId x, AggregateId y)
        {
            return x.Value.Equals(y.Value);
        }

        public int GetHashCode(AggregateId obj)
        {
            return obj.GetHashCode();
        }
    }
}