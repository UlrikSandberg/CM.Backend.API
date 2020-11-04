using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.Notification.ValueObjects;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.EnumOptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Notification.Commands
{
    public class CreateNotification
    {
        public AggregateId Id { get; private set; }
        public NotificationType Type { get; private set; }
        public NotificationMethod InvokedByMethod { get; private set; }
        public NotificationAction InvokedByAction { get; private set; }
        public DateTime Date { get; private set; }
        public DateTime ExpiresOn { get; private set; }
        public AggregateId InvokerId { get; private set; }
        public Name InvokerName { get; private set; }
        public ImageId InvokerProfileImgId { get; private set; }
        public HashSet<AggregateId> Receivers { get; private set; }
        public string ProvidedContextUrl { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string OriginalMessage { get; private set; }
        public Dictionary<string, string> PlaceholdeDictionary { get; private set; }

        public CreateNotification(NotificationType type, NotificationMethod invokedByMethod, NotificationAction invokedByAction, DateTime date, DateTime expiresOn, AggregateId invokerId, Name invokerName, ImageId invokerProfileImgId, string title, string message, string originalMessage, Dictionary<string, string> placeholdeDictionary, HashSet<AggregateId> receivers = null, string providedContextUrl = null)
        {
            if (type == null || invokerId == null || invokerName == null || invokerProfileImgId == null)
            {
                throw new ArgumentException(nameof(CreateNotification) + ": Type:" + type + ", InvokerId:" + invokerId + ", InvokerName:" + invokerName + ", InvokerProfileImgId: " + invokerProfileImgId + ". One of these values are null which is not allowed");
            }
            
            Id = new AggregateId(Guid.NewGuid());
            Type = type;
            InvokedByMethod = invokedByMethod;
            InvokedByAction = invokedByAction;
            Date = date;
            ExpiresOn = expiresOn;
            InvokerId = invokerId;
            InvokerName = invokerName;
            InvokerProfileImgId = invokerProfileImgId;
            Receivers = receivers;
            ProvidedContextUrl = providedContextUrl;
            Title = title;
            Message = message;
            OriginalMessage = originalMessage;
            PlaceholdeDictionary = placeholdeDictionary;
        }
    }
}