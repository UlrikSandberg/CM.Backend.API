using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
	public class GetAllUsers : Query<IEnumerable<UserQueryLight>>
    {
        public int Page { get; private set; }
		public int PageSize { get; private set; }

		public GetAllUsers(int page, int pageSize)
        {
            PageSize = pageSize;
			Page = page;
		}
    }
}
