using System;
using System.Collections.Generic;
using System.Data.Common;
using CM.Backend.Persistence.Model;

namespace CM.Backend.Queries.Model
{
    public class ChampagneQueryModel
    {
        public Guid Id { get; set; }

        public string BottleName { get; set; }
        public Guid BrandId { get; set; }
        public string BrandName { get; set; }
        public string BrandProfileText { get; set; }
        public Guid BottleCoverImgId { get; set; }
        public Guid BrandCoverImgId { get; set; }
        public Guid BottleImgId { get; set; }
        public bool IsPublished { get; set; }
        
        public Guid ChampagneRootId { get; set; }
        public bool RootIsSingleton { get; set; }

        public int NumberOfTastings { get; set; }
        public double RatingSumOfTastings { get; set; }
        public double AverageRating { get; set; }
        
        // Inner classes
        public VintageInfo vintageInfo { get; set; }
        public ChampagneProfile champagneProfile { get; set; }
        public FilterSearchParameters filterSearchParameters { get; set; }
        
        public bool IsBookmarkedByRequester { get; set; }
        public bool IsTastedByRequester { get; set; }
        
        public Tasting RequesterTasting { get; set; }

        public class VintageInfo
        {
            public bool IsVintage { get; set; }
            public int? Year { get; set; }
        }      

        public class ChampagneProfile
        {
            public string Appearance { get; set; }
            public string BlendInfo { get; set; }
            public string Taste { get; set; }
            public string FoodPairing { get; set; }
            public string Aroma { get; set; }
            public string BottleHistory { get; set; }
            public string Dosage { get; set; }
            public string Style { get; set; }
            public string Character { get; set; }
            public double RedWineAmount { get; set; }
            public double ServingTemp { get; set; }
            public double AgeingPotential { get; set; }
            public double ReserveWineAmount { get; set; }
            public double DosageAmount { get; set; }
            public double AlchoholVol { get; set; }
            public double PinotNoir { get; set; }
            public double PinotMeunier { get; set; }
            public double Chardonnay { get; set; }
        }

        public class FilterSearchParameters
        {
            public string Dosage { get; set; }
            public HashSet<string> Style { get; set; }
            public HashSet<string> Character { get; set; }
        }
    }
}