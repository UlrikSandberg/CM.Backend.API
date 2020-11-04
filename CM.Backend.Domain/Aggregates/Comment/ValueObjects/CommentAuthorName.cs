using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Comment.ValueObjects
{
    public class CommentAuthorName : SingleValueObject<string>
    {
        public CommentAuthorName(string value) : base(value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(nameof(value));
            }
        }
    }
}