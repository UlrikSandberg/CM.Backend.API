using System;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class UpdateListType : Command
    {
        public Guid AuthorId { get; }
        public Guid ListId { get; }
        public string ListType { get; }

        public UpdateListType(Guid authorId, Guid listId, string listType)
        {
            AuthorId = authorId;
            ListId = listId;
            ListType = listType;
        }
    }
}