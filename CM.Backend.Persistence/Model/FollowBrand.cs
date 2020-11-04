using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class FollowBrand : IEntity
    {
		
        public Guid Id { get; set; }

        public Guid FollowByUserId { get; set; }
        public string FollowByUserName { get; set; }
        public Guid FollowByUserProfileImgId { get; set; }

        public Guid FollowToBrandId { get; set; }
        public string FollowToBrandName { get; set; }
        public Guid FollowToBrandLogoImgId { get; set; }

		[BsonId]
		public PrimaryKey Key { get; set; }

		public class PrimaryKey
        {
			public Guid FollowByUserId { get; set; }
			public Guid FollowToBrandId { get; set; }
        }
    }
}
