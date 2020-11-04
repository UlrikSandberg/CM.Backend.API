using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.UserLists.Entities;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class FeaturedScheduleSet : DomainEvent
    {
        public FeaturedSchedule FeaturedSchedule { get; }

        public FeaturedScheduleSet(Guid id, FeaturedSchedule featuredSchedule) : base(id)
        {
            FeaturedSchedule = featuredSchedule;
        }
    }
}