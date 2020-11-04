using System;
using CM.Backend.Documents.Events;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class UserNotificationSettingsUpdated : DomainEvent
    {
        public bool ReceiveCmNotifications { get; private set; }
        public bool NotifyNewFollower { get; private set; }
        public bool NotifyNewComment { get; private set; }
        public bool NotifyActivityInThread { get; private set; }
        public bool NotifyLikeTasting { get; private set; }
        public bool NotifyLikeComment { get; private set; }
        public bool ReceiveNewsLetter { get; private set; }
        public bool NotifyChampagneTasted { get; private set; }
        public bool NotifyBrandNews { get; private set; }

        public UserNotificationSettingsUpdated(Guid id, bool receiveCMNotifications, bool notifyNewFollower, bool notifyNewComment, bool notifyActivityInThread, bool notifyLikeTasting, bool notifyLikeComment, bool receiveNewsLetter, bool notifyChampagneTasted, bool notifyBrandNews) : base(id)
        {
            ReceiveCmNotifications = receiveCMNotifications;
            NotifyNewFollower = notifyNewFollower;
            NotifyNewComment = notifyNewComment;
            NotifyActivityInThread = notifyActivityInThread;
            NotifyLikeTasting = notifyLikeTasting;
            NotifyLikeComment = notifyLikeComment;
            ReceiveNewsLetter = receiveNewsLetter;
            NotifyChampagneTasted = notifyChampagneTasted;
            NotifyBrandNews = notifyBrandNews;
        }
    }
}