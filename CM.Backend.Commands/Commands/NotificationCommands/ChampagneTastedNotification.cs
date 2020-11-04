using System;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class ChampagneTastedNotification : Command
    {
        public Guid InvokerId { get; private set; }
        public string InvokerName { get; private set; }
        public Guid InvokerProfileImgId { get; private set; }
        public string BottleName { get; private set; }
        public string ContextUrl { get; private set; }

        public ChampagneTastedNotification(Guid invokerId, string invokerName, Guid invokerProfileImgId,
            string bottleName, string contextUrl)
        {
            InvokerId = invokerId;
            InvokerName = invokerName;
            InvokerProfileImgId = invokerProfileImgId;
            BottleName = bottleName;
            ContextUrl = contextUrl;
        }
    }
}