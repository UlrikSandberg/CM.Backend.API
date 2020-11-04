using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Entities
{
    public class DeviceInstallation
    {
        public DeviceInstallationId InstallationId { get; set; }
        public PushChannel PushChannel { get; set; }
        public DevicePlatform Platform { get; set; }
    }
}