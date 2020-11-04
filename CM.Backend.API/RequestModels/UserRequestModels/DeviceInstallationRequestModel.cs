using System;
using System.Collections.Generic;

namespace CM.Backend.API.RequestModels.UserRequestModels
{
    public class DeviceInstallationRequestModel
    {
        public string InstallationId { get; set; }
        public string Platform { get; set; }
        public string PushChannel { get; set; }
        public List<string> Tags { get; set; }
        public long UTCOffset { get; set; }
    }
}