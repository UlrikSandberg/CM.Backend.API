namespace CM.Backend.Domain.Aggregates.Comment.Commands
{
    public class DeleteComment
    {
        public bool IsDeleted { get; private set; }

        public DeleteComment(bool isDeleted)
        {
            IsDeleted = isDeleted;
        }
    }
}