using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class MarkNotificationAsRead 
    {
        public AggregateId NotificationId { get; private set; }

        public MarkNotificationAsRead(AggregateId notificationId)
        {
            if (notificationId == null)
            {
                throw new ArgumentException(nameof(notificationId));
            }
            
            NotificationId = notificationId;
        }
    }
}