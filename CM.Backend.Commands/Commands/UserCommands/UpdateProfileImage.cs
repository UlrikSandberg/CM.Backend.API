using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UpdateProfileImage : Command
    {
		public Guid UserId { get; private set; }
		public Guid ProfileImgId { get; private set; }

		public UpdateProfileImage(Guid userId, Guid profileImgId)
        {
            ProfileImgId = profileImgId;
			UserId = userId;
		}
    }
}
