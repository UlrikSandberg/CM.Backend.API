using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class DeregisterDeviceInstallation
    {
        public PushChannel PushChannel { get; private set; }

        public DeregisterDeviceInstallation(PushChannel pushChannel)
        {
            if (pushChannel == null)
            {
                throw new ArgumentException(nameof(pushChannel));
            }
            
            PushChannel = pushChannel;
        }
    }
}