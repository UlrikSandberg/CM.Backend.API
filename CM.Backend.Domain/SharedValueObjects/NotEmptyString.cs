using System;

namespace CM.Backend.Domain.SharedValueObjects
{
    public class NotEmptyString : SingleValueObject<string>
    {
        public NotEmptyString(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(nameof(value));
            }
        }
    }
}