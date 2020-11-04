using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Notification.ValueObjects
{
    public class NotificationType : SingleValueObject<string>
    {
        private const string BaseNotificationType = "BaseNotification";
        
        public NotificationType(string value) : base(value)
        {
            if (!string.Equals(Value, BaseNotificationType))
            {
                throw new ArgumentException(nameof(Value) + ": Incompatible NotificationType --> Only supports NotificationType --> " + BaseNotificationType);
            }
        }
    }
}