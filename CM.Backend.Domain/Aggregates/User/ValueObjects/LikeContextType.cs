using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.ValueObjects
{
    public class LikeContextType : SingleValueObject<string>
    {
        private const string Tasting = "Tasting";
        private const string Comment = "Comment";
        
        public LikeContextType(string value) : base(value)
        {
            if (!string.Equals(value, Tasting) && !string.Equals(value, Comment))
            {
                throw new ArgumentException(nameof(value) + ": LikeContextType must either be -->" + Tasting + " or " + Comment);
            }
        }
    }
}