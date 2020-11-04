using System;

namespace CM.Backend.Persistence.Model.Entities
{
    public class Time
    {
        public int Hours { get; set; }


        public Time(int hours)
        {
            if (hours > 24 || hours < 0)
            {
                throw new ArgumentException("Invalid time specified");
            }

            Hours = hours;
        }
    }
}