namespace CM.Backend.EventHandlers.Helpers
{
    public static class NotificationPlatformTemplates
    {
        public const string templateBodyAPNS = "{\"aps\":{\"alert\":\"$(messageParam)\"}}";
        public const string templateBodyAPNSWithUrl = "{\"aps\":{\"alert\" : \"$(messageParam)\", \"contexturl\" : \"$(contextUrlParam)\"}}";
        public const string templateBodyAPNSWithURlAndID = "{\"aps\":{\"alert\" : \"$(messageParam)\", \"contexturl\" : \"$(contextUrlParam)\", \"notificationId\" : \"$(notificationIdParam)\"}}";
        public const string templateBodyAPNSWithURlAndIDAndSound = "{\"aps\":{\"alert\" : \"$(messageParam)\", \"sound\" : \"$(soundParam)\", \"contexturl\" : \"$(contextUrlParam)\", \"notificationId\" : \"$(notificationIdParam)\"}}";
        
    }
}