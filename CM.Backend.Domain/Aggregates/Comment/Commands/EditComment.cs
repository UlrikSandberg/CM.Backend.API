using System;
using CM.Backend.Domain.Aggregates.Comment.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Comment.Commands
{
    public class EditComment
    {
        public CommentContent Content { get; private set; }

        public EditComment(CommentContent content)
        {
            if (content == null)
            {
                throw new ArgumentException(nameof(content));
            }
            Content = content;
        }
    }
}