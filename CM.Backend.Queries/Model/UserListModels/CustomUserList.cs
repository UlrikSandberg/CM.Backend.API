using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model.UserList.Entities;

namespace CM.Backend.Queries.Model.UserListModels
{
    public class CustomUserList
    {
        public Guid Id { get; set; }
        
        public Guid AuthorId { get; set; }
        
        public Guid AuthorImageId { get; set; }
        
        public Guid ImageId { get; set; }
        
        public string Title { get; set; }
        
        public string Subtitle { get; set; }
        
        public string Description { get; set; }
        
        public string ContentType { get; set; }
        
        public string ListType { get; set; }
        
        public string AuthorType { get; set; }
        
        public int NumberOfChampagnes { get; set; }
        
        public int Likes { get; set; }
        
        public int Comments { get; set; }
        
        public DateTime CreatedAt { get; set; }
    }
}