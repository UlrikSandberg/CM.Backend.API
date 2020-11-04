using System;

namespace CM.Backend.Commands.Commands
{
	public class CreateChampagneFolder : CommandWithIdResponse
    {
	    public string FolderName { get; private set; }
	    public Guid AuthorId { get; private set; }
	    public Guid DisplayImageId { get; private set; }
	    public string FolderContentType { get; private set; }
	    public string FolderType { get; private set; }
	    public bool IsOnDiscover { get; }

	    public CreateChampagneFolder(string folderName, Guid authorId, Guid displayImageId, string folderContentType, string folderType, bool isOnDiscover)
	    {
		    FolderName = folderName;
		    AuthorId = authorId;
		    DisplayImageId = displayImageId;
		    FolderContentType = folderContentType;
		    FolderType = folderType;
		    IsOnDiscover = isOnDiscover;
	    }
    }
}
