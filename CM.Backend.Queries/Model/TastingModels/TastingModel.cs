using System;
using System.Collections.Generic;

namespace CM.Backend.Queries.Model.TastingModels
{
    public class TastingModel
    {
        public Guid Id { get; set; }
        
        //Tasting Comment
        public string Review { get; set; }
        public double Rating { get; set; }
        
        //Tasting Author
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public Guid AuthorProfileImgId { get; set; }
        
        //Tasting details
        public Guid ChampagneId { get; set; }
        public Guid BrandId { get; set; }
        public DateTime TastedOnDate { get; set; }
        
        //Social information
        public int NumberOfComments { get; set; }
        public int NumberOfLikes { get; set; }
        public bool IsLikedByRequester { get; set; }
        public bool IsCommentedByRequester { get; set; }
        
        
        
    }
}