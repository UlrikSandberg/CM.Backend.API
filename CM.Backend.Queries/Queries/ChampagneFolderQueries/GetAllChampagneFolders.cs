using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetAllChampagneFolders : Query<IEnumerable<ChampagneFolderQueryModel>>
    {
        public int Page { get; private set; }
		public int PageSize { get; private set; }
	    public string FolderType { get; private set; }

	    public GetAllChampagneFolders(int page, int pageSize, string folderType)
        {
            PageSize = pageSize;
	        FolderType = folderType;
	        Page = page;
		}
    }
}
