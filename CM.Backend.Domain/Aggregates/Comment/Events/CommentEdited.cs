using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Comment.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Comment.Events
{
    public class CommentEdited : DomainEvent
    {
        public CommentContent Content { get; private set; }

        public CommentEdited(Guid id, CommentContent content) : base(id)
        {
            Content = content;
        }
    }
}