using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class NotificationMarkedAsRead : DomainEvent
    {
        public AggregateId NotificationId { get; private set; }

        public NotificationMarkedAsRead(Guid id, AggregateId notificationId) : base(id)
        {
            NotificationId = notificationId;
        }
    }
}