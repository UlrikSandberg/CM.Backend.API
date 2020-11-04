using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model.NotificationModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.NotificationQueries
{
    public class GetNotificationsPagedAsync : Query<IEnumerable<NotificationQueryModel>>
    {
        public Guid UserId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public GetNotificationsPagedAsync(Guid userId, int page, int pageSize)
        {
            UserId = userId;
            Page = page;
            PageSize = pageSize;
        }
    }
}