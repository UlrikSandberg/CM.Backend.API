using System;

namespace CM.Backend.Queries.Model
{
    public class FollowingQueryModel
    {
        public Guid Id { get; set; }
        
        public Guid FollowToId { get; set; }
        public string FollowToName { get; set; }
        public Guid FollowToProfileImg { get; set; }
        
        public bool IsRequesterFollowing { get; set; }
    }
}