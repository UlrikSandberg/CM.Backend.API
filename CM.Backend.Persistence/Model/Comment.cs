using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class Comment : IEntity
    {

        [BsonId]
        public Guid Id { get; set; }
        
        public Guid ContextId { get; set; }
        public string ContextType { get; set; }
        
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Guid AuthorProfileImgId { get; set; }
        
        public DateTime Date { get; set; }
        public string Content { get; set; }
        
        
        
        
    }
}