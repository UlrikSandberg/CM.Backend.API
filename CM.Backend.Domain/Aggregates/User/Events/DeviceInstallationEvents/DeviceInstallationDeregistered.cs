using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class DeviceInstallationDeregistered : DomainEvent
    {
        public PushChannel PushChannel { get; private set; }

        public DeviceInstallationDeregistered(Guid id, PushChannel pushChannel) : base(id)
        {
            PushChannel = pushChannel;
        }
    }
}