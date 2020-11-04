using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetChampagnesInFolder : Query<IEnumerable<ChampagneLight>>
    {
        public Guid ChampagneFolderId { get; }

		public GetChampagnesInFolder(Guid champagneFolderId)
        {
            ChampagneFolderId = champagneFolderId;
		}
    }
}
