using System;

namespace CM.Backend.Queries.Model.ChampagneModels
{
    public class ChampagneSearchModel
    {
        //MetaData
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string BrandName { get; set; }
        public Guid ImageId { get; set; }
        public Guid BrandId { get; set; }
        
        //IsVintage
        public bool IsVintage { get; set; }
        public int Year { get; set; }
        
        //Tastings
        public int NumberOfTastings { get; set; }
        public double AverageRating { get; set; }
    }
}