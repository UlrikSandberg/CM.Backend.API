using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
    public class Brand : IEntity
    {
        [BsonId]
        public Guid Id { get; set; }
		public bool Published { get; set; }

        public string Name { get; set; }
        public string BrandProfileText { get; set; }

        public Guid BrandCoverImgId { get; set; }
	    public Guid BrandListImgId { get; set; }
        public Guid BottleCoverImgId { get; set; }
	    public Guid LogoImgId { get; set; }
        public Guid[] ChampagneIds { get; set; } = new Guid[0];
	    public Guid[] PublishedChampagneIds { get; set; } = new Guid[0];
	    public Guid[] BrandPageIds { get; set; } = new Guid[0];

		public BrandCellar Cellar { get; set; }
        public Social Social { get; set; }
    }
}