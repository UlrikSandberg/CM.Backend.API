using System;

namespace CM.Backend.Domain.Aggregates.Brand.ValueObjects
{
    public class FacebookUrl : UrlValueObject
    {
        public FacebookUrl(string value) : base(value) //TODO : IS this rule okay?
        {
            if (value != null)
            {
                if (!value.Contains("facebook"))
                {
                    throw new ArgumentException(nameof(value) + ": facebookUrl is not valid. <facebook> is not a part of the url");
                }
            }
        }
    }
}