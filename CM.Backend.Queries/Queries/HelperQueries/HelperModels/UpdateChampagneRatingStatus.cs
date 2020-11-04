using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.HelperQueries.HelperModels
{
    public class UpdateChampagneRatingStatus : Query<bool>
    {
        public bool UpdateStatus { get; }

        public UpdateChampagneRatingStatus(bool updateStatus)
        {
            UpdateStatus = updateStatus;
        }
    }
}