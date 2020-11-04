using System;

namespace CM.Backend.Domain.Aggregates.UserLists.Entities
{
    public class FeaturedSchedule
    {
        public bool ApprovedForFeature { get; }
        public DateTime FeatureStart { get; }
        public DateTime FeatureEnd { get; }

        public FeaturedSchedule(bool approvedForFeature, DateTime featureStart, DateTime featureEnd)
        {
            //Seeing as this list has been approved to be featured in the app, the timespan in which the list will be visible on the featured
            //list must there for be configured. This implies that that featureStartDate and featureEndDate meets certain requirements.
            if (approvedForFeature)
            {
                //Feature start must be earlier than featureEnd
                if (featureStart.CompareTo(featureEnd) > 0)
                {
                    throw new ArgumentException(
                        $"When approving and setting a featuring timespan for a UserList. --> {nameof(featureStart)} value:{featureStart}, must be earlier than {nameof(featureEnd)} value: {featureEnd}");
                }
                
                //Approving a UserList for featuring the datetime for cannot be in the past thus it is interpreted as if a mistake has been made, because why ApproveForFeature and then set a date which would never allow it to be featured.
                if (DateTime.UtcNow.CompareTo(featureStart) > 0)
                {
                    throw new ArgumentException($"Approving a list to be featured requires the feature startTime to be later than the current date. Current Date:{DateTime.UtcNow} provided startDate: {featureStart}");
                }

                ApprovedForFeature = approvedForFeature;
                FeatureStart = featureStart;
                FeatureEnd = featureEnd;
            }
            else
            {
                //If not approved for feature we will not worry about the data here.
                FeatureStart = featureStart;
                FeatureEnd = featureEnd;
                ApprovedForFeature = approvedForFeature;
            }
        }
    }
}