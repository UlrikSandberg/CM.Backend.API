using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class MarkLatestNotificationSeen : Command
    {
        public Guid UserId { get; private set; }
        public Guid NotificationId { get; private set; }

        public MarkLatestNotificationSeen(Guid userId, Guid notificationId)
        {
            UserId = userId;
            NotificationId = notificationId;
        }
    }
}