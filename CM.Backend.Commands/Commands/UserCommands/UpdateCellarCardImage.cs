using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UpdateCellarCardImage : Command
    {
		public Guid UserId { get; private set; }
		public Guid CellarCardImgId { get; private set; }

		public UpdateCellarCardImage(Guid userId, Guid cellarCardImgId)
        {
            CellarCardImgId = cellarCardImgId;
			UserId = userId;
		}
    }
}
