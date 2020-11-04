using System;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateSleepSettings
    {
        public UTCOffSet UTCOffset { get; private set; }
        public Time NotifyFrom { get; private set; }
        public Time NotifyTo { get; private set; }

        public UpdateSleepSettings(UTCOffSet utcOffset, Time notifyFrom, Time notifyTo)
        {
            if (utcOffset == null || notifyFrom == null || notifyTo == null)
            {
                throw new ArgumentException(nameof(UpdateSleepSettings) + ": Parameter values may not be null");
            }
            
            UTCOffset = utcOffset;
            NotifyFrom = notifyFrom;
            NotifyTo = notifyTo;
        }
    }
}