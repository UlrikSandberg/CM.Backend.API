using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class UserCellar : IEntity
    {
        
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        public Guid ChampagneId { get; set; }
        public Guid TastingId { get; set; }

        [BsonId]
        public Key PrimaryKey { get; set; }
        
        //public bool IsCustomGroup { get; set; }
        //public HashSet<Guid> GroupContent { get; set; }

        public class Key
        {
            public Guid UserId { get; set; }
            public Guid ChampagneId { get; set; }
        }
    }
}