namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class DeleteUserList
    {
        public bool IsDeleted { get; }

        public DeleteUserList(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}