using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class Tasting : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
        
        //Tasting content
        public string Review { get; set; }
        public double Rating { get; set; }
        
        public Guid AuthorId { get; set; }
        public Guid ChampagneId { get; set; }
        public Guid BrandId { get; set; }
        
        public DateTime TastedOnDate { get; set; }
        
        //A tasting would further have some knowledge of the people whom have commented on this post in a one to many relationship
        //A comment can have one tasting while a tasting can have many comments.
        
    }
}