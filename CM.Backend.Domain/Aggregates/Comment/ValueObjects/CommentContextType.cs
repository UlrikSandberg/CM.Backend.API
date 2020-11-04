using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Comment.ValueObjects
{
    public class CommentContextType : SingleValueObject<string>
    {
        private const string Tasting = "Tasting";
        private const string Comment = "Comment";
        
        public CommentContextType(string value) : base(value)
        {
            if (!string.Equals(value, Tasting) && !string.Equals(value, Comment))
            {
                throw new ArgumentException(nameof(CommentContextType) + ": Incompatible CommentContextType, valid types are: " + Tasting + ", " + Comment);
            }
        }
    }
}