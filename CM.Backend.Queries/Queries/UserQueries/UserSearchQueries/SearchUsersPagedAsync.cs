using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class SearchUsersPagedAsync : Query<IEnumerable<FollowingQueryModel>>
    {
        public Guid RequestingUserId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public SearchUsersPagedAsync(Guid requestingUserId, int page, int pageSize)
        {
            RequestingUserId = requestingUserId;
            Page = page;
            PageSize = pageSize;
        }
    }
}