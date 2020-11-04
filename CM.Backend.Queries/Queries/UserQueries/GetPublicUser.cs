using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class GetPublicUser : Query<PublicUserQueryModel>
    {
        public Guid UserId { get; private set; }
        public Guid RequestingUserId { get; private set; }

        public GetPublicUser(Guid userId, Guid requestingUserId)
        {
            UserId = userId;
            RequestingUserId = requestingUserId;
        }
    }
}