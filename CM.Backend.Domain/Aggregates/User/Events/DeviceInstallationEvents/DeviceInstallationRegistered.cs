using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.Entities;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class DeviceInstallationRegistered : DomainEvent
    {
        public DeviceInstallation DeviceInstallation { get; private set; }

        public DeviceInstallationRegistered(Guid id, DeviceInstallation deviceInstallation) : base(id)
        {
            DeviceInstallation = deviceInstallation;
        }
    }
}