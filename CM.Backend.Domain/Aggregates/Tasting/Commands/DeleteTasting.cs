namespace CM.Backend.Domain.Aggregates.Tasting.Commands
{
    public class DeleteTasting
    {
        public bool IsDeleted { get; private set; }

        public DeleteTasting(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}