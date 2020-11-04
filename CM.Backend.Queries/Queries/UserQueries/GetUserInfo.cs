using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class GetUserInfo : Query<UserInfoQueryModel>
    {
        public Guid UserId { get; private set; }

        public GetUserInfo(Guid userId)
        {
            UserId = userId;
        }
    }
}