using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class BookmarkChampagne : Command
    {
		public Guid UserId { get; private set; }
		public Guid ChampagneId { get; private set; }

		public BookmarkChampagne(Guid userId, Guid champagneId)
        {
            ChampagneId = champagneId;
			UserId = userId;
		}
    }
}
