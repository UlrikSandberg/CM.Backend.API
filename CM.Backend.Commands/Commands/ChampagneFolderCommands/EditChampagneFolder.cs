using System;
namespace CM.Backend.Commands.Commands
{
	public class EditChampagneFolder : Command
    {
	    public Guid AuthorId { get; private set; }
	    public Guid ChampagneFolderId { get; private set; }
	    public string FolderName { get; private set; }
	    public Guid DisplayImageId { get; private set; }
	    public string FolderContentType { get; private set; }
	    public bool IsOnDiscover { get; }


	    public EditChampagneFolder(Guid authorId, Guid champagneFolderId, string folderName, Guid displayImageId, string folderContentType, bool isOnDiscover)
	    {
		    AuthorId = authorId;
		    ChampagneFolderId = champagneFolderId;
		    FolderName = folderName;
		    DisplayImageId = displayImageId;
		    FolderContentType = folderContentType;
		    IsOnDiscover = isOnDiscover;
	    }
    }
}
