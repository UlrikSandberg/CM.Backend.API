using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class BugAndFeedback : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool MayBeContacted { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public Guid ImageId { get; set; }
        public DateTime SubmittedDate { get; set; }
    }
}