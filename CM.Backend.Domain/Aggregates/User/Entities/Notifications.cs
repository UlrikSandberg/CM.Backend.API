namespace CM.Backend.Domain.Aggregates.User.Entities
{
    public class Notifications
    {
        public bool ReceiveNewsLetter { get; set; }
		public bool ReceiveCMNotifications { get; set; }
        
        public bool NotifyNewFollower { get; set; }
        public bool NotifyNewComment { get; set; }
        public bool NotifyActivityInThread { get; set; }
        public bool NotifyLikeTasting { get; set; }
        public bool NotifyChampagneTasted { get; set; }
        public bool NotifyBrandNews { get; set; }
        public bool NotifyLikeComment { get; set; }
    }
}
