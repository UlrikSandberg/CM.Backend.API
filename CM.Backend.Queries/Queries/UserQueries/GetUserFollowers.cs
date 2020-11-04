using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class GetUserFollowers : Query<IEnumerable<FollowersQueryModel>>
    {
        public Guid ReqUserId { get; private set; }
        public Guid UserId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public GetUserFollowers(Guid reqUserId, Guid userId, int page, int pageSize)
        {
            ReqUserId = reqUserId;
            UserId = userId;
            Page = page;
            PageSize = pageSize;
        }
    }
}