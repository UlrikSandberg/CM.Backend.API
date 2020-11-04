using System;
using CM.Backend.Domain.Aggregates.BugAndFeedback.Commands;
using CM.Backend.Domain.Aggregates.BugAndFeedback.Events;
using CM.Backend.Domain.Aggregates.BugAndFeedback.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BugAndFeedback
{
    public class BugAndFeedback : Aggregate
    {
        
        public AggregateId UserId { get; set; } 
        public BugAndFeedbackType Type { get; set; } 
        public string Content { get; set; }
        public bool MayBeContacted { get; set; }
        public ImageId ImageId { get; set; } 


        public void Execute(SubmitBugAndFeedback cmd)
        {
            RaiseEvent(new BugAndFeedbackSubmitted(
                cmd.Id.Value, cmd.UserId, cmd.MayBeContacted, cmd.Type, cmd.Content, cmd.ImageId));
        }
        
        
        protected override void RegisterHandlers()
        {
            Handle<BugAndFeedbackSubmitted>(evt =>
            {
                Id = evt.Id;
                UserId = evt.UserId;
                Type = evt.Type;
                Content = evt.Content;
                MayBeContacted = evt.MayBeContacted;
                ImageId = evt.Imageid;
            });
        }
    }
}