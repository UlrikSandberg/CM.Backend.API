using System;

namespace CM.Backend.Queries.Model
{
    public class FollowingBrandQueryModel
    {
        public Guid Id { get; set; }
        
        public Guid FollowToBrandId { get; set; }
        public string FollowToBrandName { get; set; }
        public Guid FollowToBrandLogoImgId { get; set; }
        
        public bool IsRequesterFollowing { get; set; }
    }
}