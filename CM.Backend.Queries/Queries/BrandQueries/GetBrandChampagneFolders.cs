using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetBrandChampagneFolders : Query<IEnumerable<ChampagneFolderQueryModel>>
    {
		public Guid BrandId { get; private set; }
		public bool IsEmptyFoldersIncluded { get; private set; }

		public GetBrandChampagneFolders(Guid brandId, bool isEmptyFoldersIncluded)
        {
            IsEmptyFoldersIncluded = isEmptyFoldersIncluded;
			BrandId = brandId;
		}
    }
}
