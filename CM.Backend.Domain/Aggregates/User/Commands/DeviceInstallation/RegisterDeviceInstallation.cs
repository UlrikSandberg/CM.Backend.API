using System;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class RegisterDeviceInstallation
    {
        public DeviceInstallation DeviceInstallation { get; }

        public RegisterDeviceInstallation(PushChannel pushChannel, DevicePlatform platform)
        {
            if (pushChannel == null || platform == null)
            {
                throw new ArgumentException(nameof(RegisterDeviceInstallation));
            }
            
            DeviceInstallation = new DeviceInstallation
            {
                InstallationId = new DeviceInstallationId(pushChannel.Value),//pushChannel,
                Platform = platform,
                PushChannel = pushChannel
            };
        }
    }
}