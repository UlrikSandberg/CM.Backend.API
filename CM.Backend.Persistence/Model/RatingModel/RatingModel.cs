using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model.RatingModel
{
    public class RatingModel : IEntity
    {
        /// <summary>
        /// Seeing as there will be many ratings associated with the Id of a certain champagne brand or location the key, must consist of id of the entity rated as well as id on the person who rated it
        /// </summary>
        [BsonId]
        public PrimaryKey Key { get; set; }
        
        /// <summary>
        /// Id of the object which was rated, champagneId, brandId, locationId
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Id of the contextId, in which context was this rating given --> TastingId the rating was given in coherence with a tasting, locationReviewId,
        /// </summary>
        public Guid ContextId { get; set; }
        
        /// <summary>
        /// Id of the user who submitted the rating
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// The actual rating from 0.5-5
        /// </summary>
        public double Rating { get; set; }
        
        /// <summary>
        /// Type of object rated as a string --> Champagne, Location, brand etc...
        /// </summary>
        public string Type { get; set; }
        
        /// <summary>
        /// Date of creation
        /// </summary>
        public DateTime CreatedDate { get; set; }
        
        /// <summary>
        /// Date of last update
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }


        public class PrimaryKey
        {
            /// <summary>
            /// The Id of the entity which was rated
            /// </summary>
            public Guid EntityId { get; set; }
            
            /// <summary>
            /// Id of user who rated the entity
            /// </summary>
            public Guid UserId { get; set; }
        }
    }
}