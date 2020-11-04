using System;

namespace CM.Backend.Domain.Aggregates.Brand.ValueObjects
{
    public class InstagramUrl : UrlValueObject
    { 
        public InstagramUrl(string value) : base(value) //TODO : IS this rule okay?
        {
            if (value != null)
            {
                if (!value.Contains("instagram"))
                {
                    throw new ArgumentException(nameof(value) + ": instagram is not valid. <instagram> is not a part of the url");
                }
            }
        }
    }
}