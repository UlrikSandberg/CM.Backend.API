using System;

namespace CM.Backend.Domain.Aggregates.Brand.ValueObjects
{
    public class PinterestUrl : UrlValueObject
    {
        public PinterestUrl(string value) : base(value) //TODO : IS this rule okay?
        {
            if (value != null)
            {
                if (!value.Contains("pinterest"))
                {
                    throw new ArgumentException(nameof(value) + ": pinterestUrl is not valid. <pinterest> is not a part of the url");
                }
            }
        }
    }
}