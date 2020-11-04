using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects
{
    public class ImageName : SingleValueObject<string>
    {
        public ImageName(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(nameof(value));
            }
        }
    }
}