using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model.UserModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class SearchUsersByProfilenameAndUsernamePagedAsync : Query<IEnumerable<UserSearchProjectionModel>>
    {
        public string SearchText { get; }
        public int Page { get; }
        public int PageSize { get; }

        public SearchUsersByProfilenameAndUsernamePagedAsync(string searchText, int page, int pageSize)
        {
            SearchText = searchText;
            Page = page;
            PageSize = pageSize;
        }
    }
}