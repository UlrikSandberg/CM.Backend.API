using System;
namespace CM.Backend.Commands.Commands
{
	public class DeleteChampagneFolder : Command
    {
		public Guid AuthorId { get; private set; }
		public Guid ChampagneFolderId { get; private set; }

		public DeleteChampagneFolder(Guid authorId, Guid champagneFolderId)
		{
			AuthorId = authorId;
			ChampagneFolderId = champagneFolderId;
		}
    }
}
