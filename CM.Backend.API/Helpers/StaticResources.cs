using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.Azure.NotificationHubs;

namespace CM.Backend.API.Helpers
{
    public static class StaticResources
    {
        public static bool ValidateNotificationPlatform(string platform)
        {
            var validPlatforms = new HashSet<string> {"mpns", "wns", "apns", "gcm", "fcm"};

            return validPlatforms.Contains(platform);
               
        }
    }
}