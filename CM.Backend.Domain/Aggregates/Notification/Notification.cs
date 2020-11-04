using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.Notification.Commands;
using CM.Backend.Domain.Aggregates.Notification.Events;
using CM.Backend.Domain.Aggregates.Notification.ValueObjects;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.EnumOptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Notification
{
    public class Notification : Aggregate
    {
        //Notification meta data on author of the invocation
        public AggregateId InvokerId { get; set; }
        public Name InvokerName { get; set; }
        public ImageId InvokerProfileImgId { get; set; }
        
        //The date of invocation and expiryDate
        public DateTime Date { get; set; }
        public DateTime ExpiresOn { get; set; }
        
        //What internal method caused the notification
        public NotificationMethod InvokedByMethod { get; set; }
        public NotificationAction InvokedByAction { get; set; }
        //Base-notification, deceding the nature of the notification
        public NotificationType Type { get; set; }
        
        //Specify specific users who should receive this
        public HashSet<AggregateId> Receivers { get; set; }
        public string ProvidedContextUrl { get; set; }
        
        //Notification content
        public string Title { get; set; }
        public string Message { get; set; }
        
        public string OriginalMessage { get; set; }
        public Dictionary<string, string> PlaceholderDictionary { get; set; }

        public void Execute(CreateNotification cmd)
        {
            RaiseEvent(new NotificationCreated(cmd.Id.Value, cmd.Type, cmd.InvokedByMethod, cmd.InvokedByAction, cmd.Date, cmd.ExpiresOn, cmd.InvokerId, cmd.InvokerName, cmd.InvokerProfileImgId, cmd.Title, cmd.Message, cmd.OriginalMessage, cmd.PlaceholdeDictionary, cmd.Receivers, cmd.ProvidedContextUrl));
        }
        
        protected override void RegisterHandlers()
        {
            Handle<NotificationCreated>(evt =>
            {
                Id = evt.Id;
                Type = evt.Type;
                InvokedByMethod = evt.InvokedByMethod;
                InvokedByAction = evt.InvokedByAction;
                Date = evt.Date;
                ExpiresOn = evt.ExpiresOn;
                InvokerId = evt.InvokerId;
                InvokerName = evt.InvokerName;
                InvokerProfileImgId = evt.InvokerProfileImageId;
                Title = evt.Title;
                Message = evt.Message;
                Receivers = evt.Receivers;
                ProvidedContextUrl = evt.ProvidedContextUrl;
                OriginalMessage = evt.OriginalMessage;
                PlaceholderDictionary = evt.PlaceholderDictionary;
            });
        }
    }
}