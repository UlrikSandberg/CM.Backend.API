using System;
using System.Collections.Generic;
using CM.Backend.Persistence.EnumOptions;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class Notification : IEntity
    {
        public Guid Id { get; set; }
        
        public Guid InvokerId { get; set; }
        public string InvokerName { get; set; }
        public Guid InvokerProfileImgId { get; set; }
        
        //The date of invocation and expiryDate
        public DateTime Date { get; set; }
        public DateTime ExpiresOn { get; set; }
        
        //What internal method caused the notification
        public NotificationMethod InvokedByMethod { get; set; }
        public NotificationAction InvokedByAction { get; set; }
        //Base-notification, deceding the nature of the notification
        public string Type { get; set; }
        
        public string ContextUrl { get; set; }
        
        //Notification content
        public string Title { get; set; }
        public string Message { get; set; }
        
        //Notification -> Notification
        public string OriginalMessage { get; set; }
        public Dictionary<string, string> PlaceholderDictionary { get; set; }
    }
}