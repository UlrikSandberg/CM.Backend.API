using System;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class CommunityUpdateNotification : Command
    {
        public string Message { get; private set; }
        public string BroadCastMessage { get; private set; }
        public string ContextUrl { get; private set; }
        public Guid BrandId { get; private set; }

        public CommunityUpdateNotification(string message, string broadCastMessage, string contextUrl, Guid brandId)
        {
            Message = message;
            BroadCastMessage = broadCastMessage;
            ContextUrl = contextUrl;
            BrandId = brandId;
        }
    }
}