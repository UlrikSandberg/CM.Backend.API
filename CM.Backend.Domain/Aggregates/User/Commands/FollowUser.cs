using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class FollowUser
    {
		public AggregateId FollowToId { get; private set; }
		public NotEmptyString FollowToName { get; private set; }
		public ImageId FollowToImageId { get; private set; }

		public FollowUser(AggregateId followToId, NotEmptyString followToName, ImageId followToImageId)
        {
	        if (followToId == null || followToName == null || followToImageId == null)
	        {
		        throw new ArgumentException(nameof(FollowUser) + ": Parameter values may not be null");
	        }
	        
            FollowToImageId = followToImageId;
			FollowToName = followToName;
			FollowToId = followToId;
		}
    }
}
