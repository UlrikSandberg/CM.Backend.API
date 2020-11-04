using System;

namespace CM.Backend.Queries.Model
{
    public class FollowersQueryModel
    {
        
        public Guid Id { get; set; }
        
        public Guid FollowById { get; set; }
        public string FollowByName { get; set; }
        public Guid FollowByProfileImgId { get; set; }
        
        public bool IsRequesterFollowing { get; set; }
        
        
    }
}