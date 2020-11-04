using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class SearchUsersByUsername : Query<IEnumerable<FollowingQueryModel>>
    {
        public Guid RequestingUserId { get; private set; }
        public string Username { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public SearchUsersByUsername(Guid requestingUserId, string username, int page, int pageSize)
        {
            RequestingUserId = requestingUserId;
            Username = username;
            Page = page;
            PageSize = pageSize;
        }
    }
}