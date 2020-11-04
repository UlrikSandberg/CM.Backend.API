using System;
using CM.Backend.Queries.Model.NotificationModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.NotificationQueries
{
    public class GetLatestNotificationUpdateForUserAsync : Query<LatestNotificationUpdateModel>
    {
        public Guid UserId { get; private set; }
        public bool IncludeUpdates { get; private set; }
        public Guid FromId { get; private set; }

        public GetLatestNotificationUpdateForUserAsync(Guid userId, bool includeUpdates, Guid fromId)
        {
            UserId = userId;
            IncludeUpdates = includeUpdates;
            FromId = fromId;
        }
    }
}