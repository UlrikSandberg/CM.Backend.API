using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class FollowUser : Command
    {
		public Guid FollowById { get; private set; }
		public Guid FollowToId { get; private set; }

		public FollowUser(Guid followById, Guid followToId)
        {
            FollowToId = followToId;
			FollowById = followById;
		}
    }
}
