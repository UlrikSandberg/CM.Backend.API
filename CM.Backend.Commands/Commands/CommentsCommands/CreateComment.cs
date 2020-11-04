using System;

namespace CM.Backend.Commands.Commands.CommentsCommands
{
    public class CreateComment : CommandWithIdResponse
    {
        public Guid ContextId { get; private set; }
        public string ContextType { get; private set; }
        public Guid AuthorId { get; private set; }
        public DateTime Date { get; private set; }
        public string Content { get; private set; }

        public CreateComment(Guid contextId, string contextType, Guid authorId, DateTime date, string content)
        {
            ContextId = contextId;
            ContextType = contextType;
            AuthorId = authorId;
            Date = date;
            Content = content;
        }
    }
}