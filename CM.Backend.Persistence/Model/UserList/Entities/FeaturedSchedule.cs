using System;

namespace CM.Backend.Persistence.Model.UserList.Entities
{
    public class FeaturedSchedule
    {
        public bool ApprovedForFeature { get; set; }
        public DateTime FeatureStart { get; set; }
        public DateTime FeatureEnd { get; set; }
    }
}