using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class MarkLatestNotificationSeen
    {
        public AggregateId NotificationId { get; private set; }

        public MarkLatestNotificationSeen(AggregateId notificationId)
        {
            if (notificationId == null)
            {
                throw new ArgumentException(nameof(notificationId));
            }
            
            NotificationId = notificationId;
        }
    }
}