using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetChampagneFolder : Query<ChampagneFolderQueryModel>
    {
		public Guid ChampagneFolderId { get; private set; }

	    public GetChampagneFolder(Guid champagneFolderId)
	    {
		    ChampagneFolderId = champagneFolderId; 
	    }
    }
}
