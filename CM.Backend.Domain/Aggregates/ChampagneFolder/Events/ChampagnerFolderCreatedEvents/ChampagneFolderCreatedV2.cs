using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderCreatedEvents
{
    public class ChampagneFolderCreatedV2 : DomainEvent
    {
        public NotEmptyString FolderName { get; private set; }
        public AggregateId AuthorId { get; private set; }
        public ImageId DisplayImageId { get; private set; }
        public bool IsDeleted { get; private set; }
        public FolderContentType ContentType { get; private set; }
        public FolderType FolderType { get; private set; }
        public HashSet<AggregateId> ChampagneIds { get; private set; }
        public bool IsOnDiscover { get; }


        public ChampagneFolderCreatedV2(Guid id, NotEmptyString folderName, AggregateId authorId, ImageId displayImageId, bool isDeleted, FolderContentType contentType, FolderType folderType, HashSet<AggregateId> champagneIds, bool isOnDiscover) : base(id)
        {
            FolderName = folderName;
            AuthorId = authorId;
            DisplayImageId = displayImageId;
            IsDeleted = isDeleted;
            ContentType = contentType;
            FolderType = folderType;
            ChampagneIds = champagneIds;
            IsOnDiscover = isOnDiscover;
        }
    }
}