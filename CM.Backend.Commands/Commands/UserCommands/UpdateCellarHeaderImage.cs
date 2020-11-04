using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UpdateCellarHeaderImage : Command
    {
		public Guid UserId { get; private set; }
		public Guid CellarHeaderImgId { get; private set; }

		public UpdateCellarHeaderImage(Guid userId, Guid cellarHeaderImgId)
        {
            CellarHeaderImgId = cellarHeaderImgId;
			UserId = userId;
		}
    }
}
