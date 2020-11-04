using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.ValueObjects
{
    public class UrlValueObject
    {
        public string Url { get; private set; } //TODO : IS this rule oka
        
        public UrlValueObject(string value)
        {
            if (value != null)
            {
                Uri uriResult;
                if (!Uri.TryCreate(value, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttps)
                {
                    throw new ArgumentException(nameof(value) + ": provided url must be absolute and use httpsScheme");
                }
            }

            Url = value;
        }
    }
}