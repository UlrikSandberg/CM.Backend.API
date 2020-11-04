using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class BrandPageSection : IEntity
    {
        [BsonId]
		public Guid Id { get; set; }

		public Guid BrandId { get; set; }

		public string Title { get; set; }
	    
	    public string SubTitle { get; set; }

		public string Body { get; set; }

		public Guid[] Champagnes { get; set; } = new Guid[0];

		public Guid[] Images { get; set; } = new Guid[0];

    }
}
