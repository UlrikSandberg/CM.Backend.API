using System;
using System.Collections.Generic;

namespace CM.Backend.API.RequestModels.ChampagneRequestModels
{
    public class FilterSearchQueryRequestModel
    {
        
        public VintageInfo IsVintage { get; set; }
        public IEnumerable<string> ChampagneStyle { get; set; }
        public IEnumerable<string> ChampagneDosage { get; set; }

        public double LowerRating { get; set; }
        public double UpperRating { get; set; }

        public class VintageInfo
        {
            public bool Vintage { get; set; }
            public bool NonVintage { get; set; }
        }
    }
}