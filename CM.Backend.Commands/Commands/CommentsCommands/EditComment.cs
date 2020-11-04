using System;

namespace CM.Backend.Commands.Commands.CommentsCommands
{
    public class EditComment : Command
    {
        public Guid AuthorId { get; private set; }
        public Guid CommentId { get; private set; }
        public string Content { get; private set; }

        public EditComment(Guid commentId, Guid authorId, string content)
        {
            AuthorId = authorId;
            CommentId = commentId;
            Content = content;
        }
    }
}