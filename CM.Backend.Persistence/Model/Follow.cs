using System;
using MongoDB.Bson.Serialization.Attributes;

namespace CM.Backend.Persistence.Model
{
	public class Follow : IEntity
    {
        
		public Guid Id { get; set; }

		[BsonId]
		public PrimaryKey Key { get; set; }

		public Guid FollowById { get; set; }
		public string FollowByName { get; set; }
		public Guid FollowByProfileImgId { get; set; }
        
		public Guid FollowToId { get; set; }
		public string FollowToName { get; set; }
		public Guid FollowToProfileImgId { get; set; }

        public class PrimaryKey
		{
			public Guid FollowById { get; set; }
			public Guid FollowToId { get; set; }
		}


	}
}
