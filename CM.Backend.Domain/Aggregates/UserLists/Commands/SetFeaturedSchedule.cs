using System;
using CM.Backend.Domain.Aggregates.UserLists.Entities;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class SetFeaturedSchedule
    {
        public FeaturedSchedule FeaturedSchedule { get; }

        public SetFeaturedSchedule(FeaturedSchedule featuredSchedule)
        {
            if (featuredSchedule == null)
            {
                throw new ArgumentException($"One or more values in {nameof(SetFeaturedSchedule)} constructor is null");
            }
            
            FeaturedSchedule = featuredSchedule;
        }
    }
}