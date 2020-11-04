using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderEditedEvents
{
	public class ChampagneFolderEditted : DomainEvent
    {
	    public NotEmptyString FolderName { get; private set; }
	    public ImageId DisplayImageId { get; private set; }
	    public FolderContentType ContentType { get; private set; }

	    public ChampagneFolderEditted(Guid id, NotEmptyString folderName, ImageId displayImageId, FolderContentType contentType) : base(id)
	    {
		    FolderName = folderName;
		    DisplayImageId = displayImageId;
		    ContentType = contentType;
	    }
    }
}
