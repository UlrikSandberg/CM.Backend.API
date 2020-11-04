using System;

namespace CM.Backend.Persistence.Model.Entities
{
    public class SleepSettings
    {
        public long utcOffset { get; set; }
        public Time NotifyFrom { get; set; }
        public Time NotifyTo { get; set; }

        public SleepSettings()
        {
            NotifyFrom = new Time(8);
            NotifyTo = new Time(22);
        }
    }
}