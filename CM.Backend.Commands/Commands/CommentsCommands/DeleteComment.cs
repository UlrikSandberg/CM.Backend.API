using System;

namespace CM.Backend.Commands.Commands.CommentsCommands
{
    public class DeleteComment : Command
    {
        public Guid CommentId { get; private set; }
        public Guid AuthorId { get; private set; }

        public DeleteComment(Guid commentId, Guid authorId)
        {
            CommentId = commentId;
            AuthorId = authorId;
        }
    }
}