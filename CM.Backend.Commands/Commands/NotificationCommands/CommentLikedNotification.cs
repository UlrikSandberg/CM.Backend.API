using System;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class CommentLikedNotification : Command
    {
        public Guid TastingId { get; private set; }
        public Guid CommentId { get; private set; }
        public Guid CommentAuthorId { get; private set; }
        public Guid InvokerId { get; private set; }
        public string InvokerName { get; private set; }
        public Guid InvokerProfileImgId { get; private set; }
        public string BottleName { get; private set; }
        public string ContextUrl { get; private set; }

        public CommentLikedNotification(Guid tastingId, Guid commentId, Guid commentAuthorId ,Guid invokerId, string invokerName, Guid invokerProfileImgId, string bottleName, string contextUrl)
        {
            TastingId = tastingId;
            CommentId = commentId;
            CommentAuthorId = commentAuthorId;
            InvokerId = invokerId;
            InvokerName = invokerName;
            InvokerProfileImgId = invokerProfileImgId;
            BottleName = bottleName;
            ContextUrl = contextUrl;
        }
    }
}