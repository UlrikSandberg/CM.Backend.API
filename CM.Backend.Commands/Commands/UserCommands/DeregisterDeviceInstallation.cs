using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class DeregisterDeviceInstallation : Command
    {
        public Guid UserId { get; private set; }
        public string PushChannel { get; private set; }

        public DeregisterDeviceInstallation(Guid userId, string pushChannel)
        {
            UserId = userId;
            PushChannel = pushChannel;
        }
    }
}