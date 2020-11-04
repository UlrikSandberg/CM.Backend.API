using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.ValueObject
{
    public class UITemplateIdentifier : SingleValueObject<string>
    {
        private const string VintageTemplate = "VintageTemplate";
        private const string ChampagneTemplate = "ChampagneTemplate";
        
        public UITemplateIdentifier(string value) : base(value)
        {
            if (!string.Equals(value, VintageTemplate) && !string.Equals(value, ChampagneTemplate))
                throw new ArgumentException(nameof(value));
        }
    }
}