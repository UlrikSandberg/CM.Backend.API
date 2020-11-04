using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Entities
{
    public class SleepSettings
    {
        public UTCOffSet UTCOffset { get; set; } //TODO
        public Time NotifyFrom { get; set; }
        public Time NotifyTo { get; set; }

        public SleepSettings(UTCOffSet utcOffset, Time notifyFrom, Time notifyTo)
        {
            UTCOffset = utcOffset;
            NotifyFrom = notifyFrom;
            NotifyTo = notifyTo;
        }
        
        public SleepSettings(UTCOffSet utcOffset)
        {
            UTCOffset = utcOffset;
            
            NotifyFrom = new Time(8);
            NotifyTo = new Time(22);
        }

        public SleepSettings()
        {
        }
    }
}