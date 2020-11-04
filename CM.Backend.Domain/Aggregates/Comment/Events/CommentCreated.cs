using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Comment.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Comment.Events
{
    public class CommentCreated : DomainEvent
    {
        public AggregateId ContextId { get; private set; }
        public CommentContextType ContextType { get; private set; }
        public AggregateId AuthorId { get; private set; }
        public CommentAuthorName AuthorName { get; private set; }
        public ImageId AuthorProfileImgId { get; private set; }
        public DateTime Date { get; private set; }
        public CommentContent Content { get; private set; }
        public bool IsDeleted { get; private set; }

        public CommentCreated(Guid id, AggregateId contextId, CommentContextType contextType, AggregateId authorId, CommentAuthorName authorName, ImageId authorProfileImgId, DateTime date, CommentContent content, bool isDeleted) : base(id)
        {
            ContextId = contextId;
            ContextType = contextType;
            AuthorId = authorId;
            AuthorName = authorName;
            AuthorProfileImgId = authorProfileImgId;
            Date = date;
            Content = content;
            IsDeleted = isDeleted;
        }
    }
}