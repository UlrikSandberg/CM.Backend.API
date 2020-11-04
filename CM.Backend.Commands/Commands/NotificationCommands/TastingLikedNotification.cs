using System;
using CM.Backend.Documents.Responses;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class TastingLikedNotification : Command
    {
        public Guid TastingId { get; private set; }
        public Guid TastingAuthorId { get; private set; }
        public Guid InvokerId { get; private set; }
        public string InvokerName { get; private set; }
        public Guid InvokerProfileImgId { get; private set; }
        public string BottleName { get; private set; }
        public string ContextUrl { get; private set; }

        public TastingLikedNotification(Guid tastingId, Guid tastingAuthorId ,Guid invokerId, string invokerName, Guid invokerProfileImgId, string bottleName, string contextUrl)
        {
            TastingId = tastingId;
            TastingAuthorId = tastingAuthorId;
            InvokerId = invokerId;
            InvokerName = invokerName;
            InvokerProfileImgId = invokerProfileImgId;
            BottleName = bottleName;
            ContextUrl = contextUrl;
        }
    }
}