using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetAllBrands : Query<IEnumerable<BrandLight>>
    {
        public int Page { get; private set; }
        public int PageSize { get; private set; }
		public bool IncludeUnpublished { get; private set; }
	    public bool SortAscending { get; }

	    public GetAllBrands(int page, int pageSize, bool includeUnpublished, bool sortAscending)
        {
			Page = page;
			PageSize = pageSize;
			IncludeUnpublished = includeUnpublished;
	        SortAscending = sortAscending;
        }
    }
}
