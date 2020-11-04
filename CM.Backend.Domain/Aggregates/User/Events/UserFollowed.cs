using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;
using SimpleSoft.Mediator;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class UserFollowed : DomainEvent
	{
		public AggregateId FollowToId { get; private set; }
		public NotEmptyString FollowToName { get; private set; }
		public ImageId FollowToImageId { get; private set; }
		public NotEmptyString FollowByName { get; private set; }
		public ImageId FollowByImageId { get; private set; }

		public UserFollowed(Guid id, AggregateId followToId, NotEmptyString followToName, ImageId followToImageId, NotEmptyString followByName, ImageId followByImageId) : base(id)
        {
            FollowByImageId = followByImageId;
			FollowByName = followByName;
			FollowToImageId = followToImageId;
			FollowToName = followToName;
			FollowToId = followToId;
		}
    }
}
