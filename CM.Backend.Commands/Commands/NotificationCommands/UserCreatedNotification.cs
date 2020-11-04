using System;
using System.Net.Http;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class UserCreatedNotification : Command
    {
        public Guid InvokerId { get; private set; }
        public string InvokerName { get; private set; }

        public UserCreatedNotification(Guid invokerId, string invokerName = null)
        {
            InvokerId = invokerId;
            InvokerName = invokerName;
        }
    }
}