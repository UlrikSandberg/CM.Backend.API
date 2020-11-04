using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model.TastingModels;

namespace CM.Backend.Queries.Model
{
    public class ChampagneWithRatingAndTasting
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        
        public string BottleName { get; set; }
        public string BrandName { get; set; }
        
        public List<double> Ratings { get; set; }
        
        public bool IsBookmarkedByRequester { get; set; }
        public bool IsTastedByRequester { get; set; }
        
        public IEnumerable<TastingModel> Tastings { get; set; }
    }
}