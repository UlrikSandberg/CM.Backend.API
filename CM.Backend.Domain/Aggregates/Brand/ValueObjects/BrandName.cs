using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.ValueObjects
{
    public class BrandName : SingleValueObject<string>
    {
        public BrandName(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(nameof(value) + ": BrandName is not allowed to be null or empty");
            }
        }
    }
}