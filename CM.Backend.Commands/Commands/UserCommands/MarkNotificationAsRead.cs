using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class MarkNotificationAsRead : Command
    {
        public Guid UserId { get; private set; }
        public Guid NotificationId { get; private set; }

        public MarkNotificationAsRead(Guid userId, Guid notificationId)
        {
            UserId = userId;
            NotificationId = notificationId;
        }
    }
}