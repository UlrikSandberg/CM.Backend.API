using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
	public class GetUser : Query<UserQueryModel>
    {
        public Guid UserId { get; private set; }

		public GetUser(Guid userId)
        {
            UserId = userId;
		}
    }
}
