using System;
using Baseline;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class SetFeaturedSchedule : Command
    {
        public Guid ListId { get; }
        public bool IsApprovedForFeature { get; }
        public DateTime FeatureStart { get; }
        public DateTime FeatureEnd { get; }

        public SetFeaturedSchedule(Guid listId, bool isApprovedForFeature, DateTime featureStart, DateTime featureEnd)
        {
            ListId = listId;
            IsApprovedForFeature = isApprovedForFeature;
            FeatureStart = featureStart;
            FeatureEnd = featureEnd;
        }
    }
}