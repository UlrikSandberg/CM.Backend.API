using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class GetUserCellarPaged : Query<IEnumerable<UserCellarChampagneQueryModel>>
    {
        public Guid UserId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public GetUserCellarPaged(Guid userId, int page, int pageSize)
        {
            UserId = userId;
            Page = page;
            PageSize = pageSize;
        }
    }
}