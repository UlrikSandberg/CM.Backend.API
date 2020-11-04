using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Events
{
    public class ChampagneAddedToFolder : DomainEvent
    {
        public AggregateId ChampagneId { get; private set; }
        public FolderType FolderType { get; private set; }
        public FolderContentType FolderContentType { get; private set; }

        public ChampagneAddedToFolder(Guid id, AggregateId champagneId, FolderType folderType, FolderContentType folderContentType) : base(id)
        {
            ChampagneId = champagneId;
            FolderType = folderType;
            FolderContentType = folderContentType;
        }
    }
}