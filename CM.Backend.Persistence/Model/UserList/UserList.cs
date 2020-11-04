using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model.UserList.Entities;

namespace CM.Backend.Persistence.Model.UserList
{
    public class UserList : IEntity
    {
        public Guid Id { get; set; }
        
        public Guid AuthorId { get; set; }
        
        public Guid ImageId { get; set; }
        
        public string Title { get; set; }
        
        public string Subtitle { get; set; }
        
        public string Description { get; set; }
        
        public string ContentType { get; set; }
        
        public string ListType { get; set; }
        
        public string AuthorType { get; set; }
        
        public List<Guid> ListContent { get; set; }
        
        public FeaturedSchedule FeaturedSchedule { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime LastEditedAt { get; set; }
    }
}