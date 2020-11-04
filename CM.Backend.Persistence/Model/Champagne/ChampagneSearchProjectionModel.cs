using System;
using System.Collections.Generic;

namespace CM.Backend.Persistence.Model
{
    public class ChampagneSearchProjectionModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ImageId { get; set; }
        public Guid BrandId { get; set; }
        
        //IsVintage
        public bool IsVintage { get; set; }
        public int Year { get; set; }
        
        //Tastings
        public Dictionary<string, double> RatingDictionary { get; set; }
        public double AverageRating { get; set; }
    }
}