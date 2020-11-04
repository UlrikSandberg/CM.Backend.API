using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class UserUnfollowed : DomainEvent
    {
		public AggregateId FollowToId { get; private set; }

		public UserUnfollowed(Guid id, AggregateId followToId) : base(id)
        {
            FollowToId = followToId;
		}
    }
}
