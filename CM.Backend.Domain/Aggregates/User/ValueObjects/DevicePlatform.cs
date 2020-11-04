using System;
using System.Collections.Generic;
using System.Linq;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.ValueObjects
{
    public class DevicePlatform : SingleValueObject<string>
    {

        private HashSet<string> validDevicePlatforms = new HashSet<string> {"mpns", "wns", "apns", "gcm", "fcm"};
        
        public DevicePlatform(string value) : base(value)
        {
            if (!validDevicePlatforms.Contains(value))
            {
                throw new ArgumentException(nameof(value) +  ":Provided notification platform: " + value +
                ". Is not supported only -> mpns, wns, apns, gcm, fcm are valid providers");
            }
        }
    }
}