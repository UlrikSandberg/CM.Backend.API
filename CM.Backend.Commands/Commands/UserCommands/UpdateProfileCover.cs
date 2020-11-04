using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UpdateProfileCover : Command
    {
		public Guid UserId { get; private set; }
		public Guid ProfileCoverImg { get; private set; }

		public UpdateProfileCover(Guid userId, Guid profileCoverImg)
        {
            ProfileCoverImg = profileCoverImg;
			UserId = userId;
		}
    }
}
