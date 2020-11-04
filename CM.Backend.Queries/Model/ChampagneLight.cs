using System;
namespace CM.Backend.Queries.Model
{
    public class ChampagneLight
    {
        
		public Guid Id { get; set; }
		public string BottleName { get; set; }
	    public string BrandName { get; set; }
		public Guid BrandId { get; set; }
		public Guid BottleImgId { get; set; }
		public bool IsPublished { get; set; }
		public Guid ChampagneRootId { get; set; }

		public double NumberOfTastings { get; set; }
		public double RatingSumOfTastings { get; set; }

		public VintageInfo GetVintageInfo { get; set; }

        public class VintageInfo
		{
			public bool IsVintage { get; set; }
			public int? Year { get; set; }
		}
    }
}
