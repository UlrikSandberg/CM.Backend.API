using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class SearchUsersPagedAsyncLight : Query<IEnumerable<UserSearchProjectionModel>>
    {
        public int Page { get; }
        public int PageSize { get; }

        public SearchUsersPagedAsyncLight(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }
    }
}