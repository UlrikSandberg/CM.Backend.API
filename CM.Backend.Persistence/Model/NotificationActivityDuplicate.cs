using System;
using System.Data.Common;
using CM.Backend.Persistence.EnumOptions;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class NotificationActivityDuplicate : IEntity
    {
        [BsonId]
        public PrimaryKey Key { get; set; }
        
        public Guid Id { get; set; }
        public NotificationMethod NotificationMethod { get; set; }
        
        public class PrimaryKey
        {
            public Guid InvokedById { get; set; }
            public Guid InvokedOnId { get; set; }
        }
    }
}