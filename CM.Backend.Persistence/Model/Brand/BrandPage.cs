using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class BrandPage : IEntity
    {
		[BsonElement]
		public Guid Id { get; set; }

		public Guid BrandId { get; set; }

		public string Title { get; set; }

		public bool Published { get; set; }
	    
	    public string UITemplateIdentifier { get; set; }

		public Guid CardImgId { get; set; }

		public Guid HeaderImgId { get; set; }

		public string Url { get; set; }

		public Guid[] SectionIds { get; set; }


    }
}
