using System.Collections.Generic;

namespace CM.Backend.Queries.Model.NotificationModels
{
    public class LatestNotificationUpdateModel
    {
        public int NumberOfUnreadNotifications { get; set; }
        public IEnumerable<NotificationQueryModel> NewNotifications { get; set; }
    }
}