using System;

namespace CM.Backend.Commands.Commands.BugAndFeedbackCommands
{
    public class SubmitBugOrFeedback : Command
    {
        public Guid UserId { get; private set; }
        public string Type { get; private set; }
        public string Content { get; private set; }
        public Guid ImageId { get; private set; }
        public bool MayBeContacted { get; private set; }


        public SubmitBugOrFeedback(Guid userId, string type, string content, Guid imageId, bool mayBeContacted)
        {
            UserId = userId;
            Type = type;
            Content = content;
            ImageId = imageId;
            MayBeContacted = mayBeContacted;
        }
        
    }
}