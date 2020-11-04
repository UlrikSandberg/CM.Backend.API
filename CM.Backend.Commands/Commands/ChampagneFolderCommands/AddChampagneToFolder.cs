using System;

namespace CM.Backend.Commands.Commands
{
    public class AddChampagneToFolder : Command
    {
        public Guid AuthorId { get; private set; }
        public Guid ChampagneFolderId { get; private set; }
        public Guid ChampagneId { get; private set; }

        public AddChampagneToFolder(Guid authorId, Guid champagneFolderId, Guid champagneId)
        {
            AuthorId = authorId;
            ChampagneFolderId = champagneFolderId;
            ChampagneId = champagneId;
        }
    }
}