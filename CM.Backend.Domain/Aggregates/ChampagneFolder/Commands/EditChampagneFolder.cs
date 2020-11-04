using System;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Commands
{
    public class EditChampagneFolder
    {
	    public NotEmptyString FolderName { get; private set; }
	    public ImageId DisplayImageId { get; private set; }
	    public FolderContentType ContentType { get; private set; }
	    public bool IsOnDiscover { get; }

	    public EditChampagneFolder(NotEmptyString folderName, ImageId displayImageId, FolderContentType contentType, bool isOnDiscover)
	    {
		    if (folderName == null || displayImageId == null || contentType == null)
		    {
			    throw new ArgumentException(nameof(EditChampagneFolder));
		    }
		    
		    FolderName = folderName;
		    DisplayImageId = displayImageId;
		    ContentType = contentType;
		    IsOnDiscover = isOnDiscover;
	    }
    }
}
