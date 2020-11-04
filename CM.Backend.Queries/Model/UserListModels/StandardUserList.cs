using System;
using System.Collections.Generic;

namespace CM.Backend.Queries.Model.UserListModels
{
    public class StandardUserList
    {
        public Guid ImageId { get; set; }
        
        public string Title { get; set; }
        
        public string Subtitle { get; set; }
        
        public string Description { get; set; }
        
        public string ContentType { get; set; }
        
        public string ConfigurationKey { get; set; }
        
        public bool IsValidForNonVintage { get; set; }
        
        public bool IsValidForVintage { get; set; }
    }
}