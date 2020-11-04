using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class RegisterDeviceInstallation : Command
    {
        public Guid UserId { get; private set; }
        public string PushChannel { get; private set; }
        public string NotificationPlatform { get; private set; }

        public RegisterDeviceInstallation(Guid userId, string pushChannel, string notificationPlatform)
        {
            UserId = userId;
            PushChannel = pushChannel;
            NotificationPlatform = notificationPlatform;
        }
    }
}