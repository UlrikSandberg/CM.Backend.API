using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UnfollowUser
    {
        public AggregateId FollowToId { get; private set; }

		public UnfollowUser(AggregateId followToId)
        {
            if (followToId == null)
            {
                throw new ArgumentException(nameof(followToId));
            }
            
            FollowToId = followToId;
		}
    }
}
