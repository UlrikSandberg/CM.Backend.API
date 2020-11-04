using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UnfollowUser : Command
    {
		public Guid FollowById { get; private set; }
		public Guid FollowToId { get; private set; }

		public UnfollowUser(Guid followById, Guid followToId)
        {
            FollowToId = followToId;
			FollowById = followById;
		}
    }
}
