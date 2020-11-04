using System;

namespace CM.Backend.Commands.Commands
{
    public class RemoveChampagneFromFolder : Command
    {
        public Guid AuthorId { get; private set; }
        public Guid ChampagneFolderId { get; private set; }
        public Guid ChampagneId { get; private set; }

        public RemoveChampagneFromFolder(Guid authorId, Guid champagneFolderId, Guid champagneId)
        {
            AuthorId = authorId;
            ChampagneFolderId = champagneFolderId;
            ChampagneId = champagneId;
        }
    }
}