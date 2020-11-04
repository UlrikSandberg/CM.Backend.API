using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class UnbookmarkChampagne : Command
    {
		public Guid UserId { get; private set; }
		public Guid ChampagneId { get; private set; }

		public UnbookmarkChampagne(Guid userId, Guid champagneId)
        {
            ChampagneId = champagneId;
			UserId = userId;
		}
    }
}
