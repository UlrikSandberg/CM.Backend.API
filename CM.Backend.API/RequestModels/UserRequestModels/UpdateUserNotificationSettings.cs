namespace CM.Backend.API.RequestModels.UserRequestModels
{
    public class UpdateUserNotificationSettingsRequestModel
    {
        public bool ReceiveCMNotifications { get; set; }
        public bool NotifyNewFollower { get; set; }
        public bool NotifyNewComment { get; set; }
        public bool NotifyActivityInThread { get; set; }
        public bool NotifyLikeTasting { get; set; }
        public bool NotifyLikeComment { get; set; }
        public bool ReceiveNewsLetter { get; set; }
        public bool NotifyChampagneTasted { get; set; }
        public bool NotifyBrandNews { get; set; }
    }
}