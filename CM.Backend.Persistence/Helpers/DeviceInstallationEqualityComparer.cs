using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;

namespace CM.Backend.Persistence.Helpers
{
    public class DeviceInstallationEqualityComparer : IEqualityComparer<Installation>
    {
        public bool Equals(Installation x, Installation y)
        {
            return x.PushChannel.Equals(y.PushChannel);
        }

        public int GetHashCode(Installation obj)
        {
            return obj.GetHashCode();
        }
    }
}