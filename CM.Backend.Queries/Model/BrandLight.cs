using System;

namespace CM.Backend.Queries.Model
{
    public class BrandLight
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
		public bool Published { get; set; }

        public Guid BrandCoverImageId { get; set; }
        public Guid BottleCoverImageId { get; set; }
        public Guid BrandListImageId { get; set; }
        public Guid BrandLogoImageId { get; set; }
        public Guid[] ChampagneIds { get; set; } = new Guid[0];
        public int NumberOfChampagnes { get; set; }

        //Since a brand should be searchable when entering the app it should support two fields for counting the number of tastings and Followers from the beginning 
        public int NumberOfTastings { get; set; }
        public int NumberOfFollowers { get; set; }
    }
}
