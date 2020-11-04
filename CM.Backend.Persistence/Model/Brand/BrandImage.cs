using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class BrandImage : IEntity
    {
		[BsonId]
		public Guid Id { get; set; }
		public Guid BrandId { get; set; }
		public string Name { get; set; }
		public string TypeOfBrandImage { get; set; }
		public string FileType { get; set; }
    }
}
