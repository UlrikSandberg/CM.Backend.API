namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class AddNotificationsSettings
    {
        public bool ReceivedCMNotifications { get; private set; }
        public bool NotifyNewFollower { get; private set; }
        public bool NotifyNewComment { get; private set; }
        public bool NotifyActivityInThread { get; private set; }
        public bool NotifyLikeTasting { get; private set; }
        public bool NotifyLikeComment { get; private set; }
        public bool ReceiveNewsLetter { get; private set; }
        public bool NotifyChampagneTasted { get; private set; }
        public bool NotifyBrandNews { get; private set; }

        public AddNotificationsSettings(bool receivedCMNotifications, bool notifyNewFollower, bool notifyNewComment, bool notifyActivityInThread, bool notifyLikeTasting, bool notifyLikeComment, bool receiveNewsLetter, bool notifyChampagneTasted, bool notifyBrandNews)
        {
            ReceivedCMNotifications = receivedCMNotifications;
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