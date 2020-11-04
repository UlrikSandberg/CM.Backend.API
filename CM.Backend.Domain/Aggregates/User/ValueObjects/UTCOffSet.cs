using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Entities
{
    public class UTCOffSet : SingleValueObject<long>
    {
        public UTCOffSet(long value) : base(value)
        {
            if (value < 0.0)
            {
                throw new ArgumentException(nameof(value) + ": UTCOffset may not be less than zero ");
            }
        }
    }
}