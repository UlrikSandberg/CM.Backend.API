using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class SearchUsersByProfilename : Query<IEnumerable<FollowingQueryModel>>
    {
        public Guid RequestingUserId { get; private set; }
        public string Profilename { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public SearchUsersByProfilename(Guid requestingUserId, string profilename, int page, int pageSize)
        {
            RequestingUserId = requestingUserId;
            Profilename = profilename;
            Page = page;
            PageSize = pageSize;
        }
    }
}