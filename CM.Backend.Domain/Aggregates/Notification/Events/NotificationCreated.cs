using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Notification.ValueObjects;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.EnumOptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Notification.Events
{
    public class NotificationCreated : DomainEvent
    {
        public NotificationType Type { get; private set; }
        public NotificationMethod InvokedByMethod { get; private set; }
        public NotificationAction InvokedByAction { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime ExpiresOn { get; private set; }
        public AggregateId InvokerId { get; private set; }
        public Name InvokerName { get; private set; }
        public ImageId InvokerProfileImageId { get; private set; }
        public string ProvidedContextUrl { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public HashSet<AggregateId> Receivers { get; private set; }
        public string OriginalMessage { get; private set; }
        public Dictionary<string, string> PlaceholderDictionary { get; private set; }

        public NotificationCreated(Guid id, NotificationType type, NotificationMethod invokedByMethod, NotificationAction invokedByAction, DateTime date, DateTime expiresOn, AggregateId invokerId, Name invokerName, ImageId invokerProfileImageId, string title, string message, string originalMessage, Dictionary<string, string> placeholderDictionary, HashSet<AggregateId> receivers, string providedContextUrl = null) : base(id)
        {
            Type = type;
            InvokedByMethod = invokedByMethod;
            InvokedByAction = invokedByAction;
            Date = date;
            ExpiresOn = expiresOn;
            InvokerId = invokerId;
            InvokerName = invokerName;
            InvokerProfileImageId = invokerProfileImageId;
            ProvidedContextUrl = providedContextUrl;
            Title = title;
            Message = message;
            OriginalMessage = originalMessage;
            PlaceholderDictionary = placeholderDictionary;
            Receivers = receivers;
        }
    }
}