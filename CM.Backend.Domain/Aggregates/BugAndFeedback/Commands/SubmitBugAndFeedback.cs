using System;
using CM.Backend.Domain.Aggregates.BugAndFeedback.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BugAndFeedback.Commands
{
    public class SubmitBugAndFeedback
    {
        public AggregateId Id { get; private set; }
        public AggregateId UserId { get; set; }
        public bool MayBeContacted { get; set; }
        public BugAndFeedbackType Type { get; set; }
        public string Content { get; set; }
        public ImageId ImageId { get; set; }
        
        public SubmitBugAndFeedback(AggregateId id, AggregateId userId, bool mayBeContacted, BugAndFeedbackType type, string content, ImageId imageId)
        {
            if (id == null || userId == null || type == null || imageId == null)
            {
                throw new ArgumentException(nameof(SubmitBugAndFeedback) + ": Parameter values may not be null");
            }
            
            Id = id;
            UserId = userId;
            MayBeContacted = mayBeContacted;
            Type = type;
            Content = content;
            ImageId = imageId;
        }
    }
}