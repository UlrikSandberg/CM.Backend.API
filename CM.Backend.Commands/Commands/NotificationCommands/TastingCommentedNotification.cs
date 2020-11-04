using System;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class TastingCommentedNotification : Command
    {
        public Guid TastingId { get; private set; }
        public Guid TastingAuthorId { get; private set; }
        public Guid CommentedById { get; private set; }
        public string CommentedByName { get; private set; }
        public Guid CommentedByProfileImgId { get; private set; }
        public string BottleName { get; private set; }
        public string ContextUrl { get; private set; }

        public TastingCommentedNotification(Guid tastingId, Guid tastingAuthorId, Guid commentedById, string commentedByName, Guid commentedByProfileImgId, string bottleName, string contextUrl)
        {
            TastingId = tastingId;
            TastingAuthorId = tastingAuthorId;
            CommentedById = commentedById;
            CommentedByName = commentedByName;
            CommentedByProfileImgId = commentedByProfileImgId;
            BottleName = bottleName;
            ContextUrl = contextUrl;
        }
    }
}