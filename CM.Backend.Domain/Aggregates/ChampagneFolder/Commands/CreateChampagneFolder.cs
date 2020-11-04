using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Commands
{
    public class CreateChampagneFolder
    {
		public AggregateId Id { get; private set; }
		public NotEmptyString FolderName { get; private set; }
		public AggregateId AuthorId { get; private set; }
		public ImageId DisplayImageId { get; private set; }
		public bool IsDeleted { get; private set; }
		public FolderContentType ContentType { get; private set; }
	    public FolderType FolderType { get; private set; }
	    public HashSet<AggregateId> ChampagneIds { get; private set; }
	    public bool IsOnDiscover { get; }

	    public CreateChampagneFolder(AggregateId id, NotEmptyString folderName, AggregateId authorId, ImageId displayImageId, FolderContentType contentType, FolderType folderType, HashSet<AggregateId> champagneIds, bool isOnDiscover)
        {
	        if (id == null || folderName == null || authorId == null || displayImageId == null || contentType == null ||
	            folderType == null || champagneIds == null)
	        {
		        throw new ArgumentException(nameof(CreateChampagneFolder) + ": Parameter values may not be null");
	        }
	        
			IsDeleted = false;
	        ContentType = contentType;
	        FolderType = folderType;
	        ChampagneIds = champagneIds;
	        IsOnDiscover = isOnDiscover;
	        Id = id;
	        FolderName = folderName;
	        AuthorId = authorId;
	        DisplayImageId = displayImageId;
	        
        }
    }
}
