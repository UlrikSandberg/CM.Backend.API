using System;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects
{
    public class FolderContentType : SingleValueObject<string>
    {
        private const string Vintage = "Vintage";
        private const string NonVintage = "Non-Vintage";
        private const string Mixed = "Mixed";
        
        public FolderContentType(string value) : base(value)
        {
            if (!string.Equals(value, Vintage) && !string.Equals(value, NonVintage) && !string.Equals(value, Mixed))
            {
                throw new ArgumentException(
                    $"{nameof(value)}: Incompatible FolderContentType. Accepted formats are: {Vintage}, {NonVintage}, {Mixed}. Value was '{value}'");
            }
        }
    }
}