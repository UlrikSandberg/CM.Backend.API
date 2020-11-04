using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects
{
    public class TypeOfBrandImage : SingleValueObject<string>
    {
        private const string Champagne = "Champagne";
        private const string Cover = "Cover";
        private const string Card = "Card";
        private const string Logo = "Logo";
        
        public TypeOfBrandImage(string value) : base(value)
        {
            if (!string.Equals(value, Champagne) && !string.Equals(value, Cover) && !string.Equals(value, Card) && !string.Equals(value, Logo))
            {
                throw new ArgumentException(nameof(value));
            }
        }
    }
}