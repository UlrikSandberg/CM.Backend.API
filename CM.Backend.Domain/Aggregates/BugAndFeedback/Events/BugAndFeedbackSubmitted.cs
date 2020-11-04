using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.BugAndFeedback.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BugAndFeedback.Events
{
    public class BugAndFeedbackSubmitted : DomainEvent
    {
        public AggregateId UserId { get; private set; }
        public bool MayBeContacted { get; private set; }
        public BugAndFeedbackType Type { get; private set; }
        public string Content { get; private set; }
        public ImageId Imageid { get; private set; }

        public BugAndFeedbackSubmitted(Guid id, AggregateId userId, bool mayBeContacted, BugAndFeedbackType type, string content, ImageId imageid) : base(id)
        {
            UserId = userId;
            MayBeContacted = mayBeContacted;
            Type = type;
            Content = content;
            Imageid = imageid;
        }
    }
}