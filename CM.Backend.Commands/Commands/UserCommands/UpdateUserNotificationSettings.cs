using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class UpdateUserNotificationSettings : Command
    {
        public Guid UserId { get; private set; }
        public bool ReceiveCMNotifications { get; private set; }
        public bool NotifyNewFollower { get; private set; }
        public bool NotifyNewComment { get; private set; }
        public bool NotifyActivityInThread { get; private set; }
        public bool NotifyLikeTasting { get; private set; }
        public bool NotifyLikeComment { get; private set; }
        public bool ReceiveNewsLetter { get; private set; }
        public bool NotifyChampagneTasted { get; private set; }
        public bool NotifyBrandNews { get; private set; }


        public UpdateUserNotificationSettings(Guid userId, bool receiveCMNotifications, bool notifyNewFollower, bool notifyNewComment, bool notifyActivityInThread, bool notifyLikeTasting, bool notifyLikeComment, bool receiveNewsLetter, bool notifyChampagneTasted, bool notifyBrandNews)
        {
            UserId = userId;
            ReceiveCMNotifications = receiveCMNotifications;
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