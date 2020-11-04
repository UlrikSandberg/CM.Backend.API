using System;

namespace CM.Backend.Domain.SharedValueObjects
{
    public class LimitedString : NullValueObject<string>
    {
        public int MaxStringLength { get; }

        public LimitedString(string value, int maxStringLength) : base(value)
        {
            MaxStringLength = maxStringLength;

            if (value != null)//Only if the string is not null will we do the check. The string can be null seeing as this can then be used as subtitle which is not currently attributes which has to be present.
            {
                if (value.Length > maxStringLength)
                {
                    throw new ArgumentException($"Length of string: {nameof(value)} exceeds the set, {nameof(LimitedString)} - {nameof(MaxStringLength)} property, with value of: {MaxStringLength}");
                }
            }
        }
    }
}