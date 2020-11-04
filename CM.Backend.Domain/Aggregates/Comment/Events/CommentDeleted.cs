using System;
using CM.Backend.Documents.Events;

namespace CM.Backend.Domain.Aggregates.Comment.Events
{
    public class CommentDeleted : DomainEvent
    {
        public bool IsDeleted { get; private set; }

        public CommentDeleted(Guid id, bool isDeleted) : base(id)
        {
            IsDeleted = isDeleted;
        }
    }
}