using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class Like : IEntity
    {
        public Guid Id { get; set; }

        [BsonId]
        public PrimaryKey Key { get; set; }
        
        public Guid LikeById { get; set; }
        public string LikeByName { get; set; }
        public Guid LikeByProfileImgId { get; set; }
        
        public Guid LikeToContextId { get; set; }
        public string ContextType { get; set; }

        public class PrimaryKey
        {
            public Guid LikeById { get; set; }
            public Guid LikeToContextId { get; set; }
        }
            
            
            
            
    }
}