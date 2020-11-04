using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
	public class GetChampagnes : Query<IEnumerable<Champagne>>
    {
        public int Page { get; private set; }
		public int PageSize { get; private set; }

		public GetChampagnes(int page, int pageSize)
        {
            PageSize = pageSize;
			Page = page;
		}
    }
}
