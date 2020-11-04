namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Commands
{
    public class DeleteChampagneFolder
    {
		public bool IsDeleted { get; }

		public DeleteChampagneFolder()
        {
            IsDeleted = true;
		}
    }
}
