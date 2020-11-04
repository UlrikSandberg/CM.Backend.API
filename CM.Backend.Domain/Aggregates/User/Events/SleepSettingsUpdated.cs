using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class SleepSettingsUpdated : DomainEvent
    {
        public UTCOffSet UTCOffset { get; private set; }
        public Time NotifyFrom { get; private set; }
        public Time NotifyTo { get; private set; }

        public SleepSettingsUpdated(Guid id, UTCOffSet utcOffset, Time notifyFrom, Time notifyTo) : base(id)
        {
            UTCOffset = utcOffset;
            NotifyFrom = notifyFrom;
            NotifyTo = notifyTo;
        }
    }
}