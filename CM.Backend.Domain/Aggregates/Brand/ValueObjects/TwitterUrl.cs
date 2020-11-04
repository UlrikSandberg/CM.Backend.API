using System;
using System.Linq;

namespace CM.Backend.Domain.Aggregates.Brand.ValueObjects
{
    public class TwitterUrl : UrlValueObject
    {
        public TwitterUrl(string value) : base(value) //TODO : IS this rule okay?
        {
            if (value != null)
            {
                if (!value.Contains("twitter"))
                {
                    throw new ArgumentException(nameof(value) + ": Twitter url not valid. <twitter> is not a part of the url");
                }
            }
        }
    }
}