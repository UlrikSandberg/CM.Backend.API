using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model.Entities;
using static CM.Backend.Persistence.Model.Brand;

namespace CM.Backend.Queries.Model
{
    public class BrandProfileExtendedBrandPage
    {
        //This model is an extention of the persistence brand model, with the difference that this class
        //holds an array of extracted BrandPageLight in order to survey app needs.

        public Guid Id { get; set; }
        public bool Published { get; set; }

        public string Name { get; set; }
        public string BrandProfileText { get; set; }

        public Guid BrandCoverImageId { get; set; }
        public Guid BottleCoverImageId { get; set; }
        public Guid BrandListImageId { get; set; }
        public Guid LogoImageId { get; set; }
        public Guid[] ChampagneIds { get; set; }
		public CellarLight Cellar { get; set; }
		public IList<BrandPageLight> Pages { get; set; } = new List<BrandPageLight>();

        public Social Social { get; set; }

        public bool IsRequesterFollowing { get; set; } = false;
        
        //Since a brand should be searchable when entering the app it should support two fields for counting the number of tastings and Followers from the beginning 
        public int NumberOfTastings { get; set; }
        public int NumberOfFollowers { get; set; }
        public int NumberOfEditions { get; set; }
    }
}
