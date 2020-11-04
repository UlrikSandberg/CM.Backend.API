using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.ValueObjects
{
    public class Time : SingleValueObject<int>
    {
        public Time(int value) : base(value)
        {
            if (value > 24 || value < 0)
            {
                throw new ArgumentException("Invalid time specified");
            }
        }
    }
}