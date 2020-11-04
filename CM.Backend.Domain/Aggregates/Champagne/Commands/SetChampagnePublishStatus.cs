namespace CM.Backend.Domain.Aggregates.Champagne.Commands
{
    public class SetChampagnePublishStatus
    {
        public bool IsPublished { get; private set; }

		public SetChampagnePublishStatus(bool isPublished)
        {
            IsPublished = isPublished;
		}
    }
}
