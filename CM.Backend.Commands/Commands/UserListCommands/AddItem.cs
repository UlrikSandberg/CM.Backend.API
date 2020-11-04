using System;
using CM.Backend.Commands.Commands.UserListCommands.Helpers;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class AddItem : Command
    {
        public Guid AuthorId { get; }
        public Guid ListId { get; }
        public Guid ItemId { get; }
        public string AuthorType { get; }

        public AddItem(Guid authorId, Guid listId, Guid itemId, string authorType = DefaultKeys.DefaultUserListAuthorType)
        {
            AuthorId = authorId;
            ListId = listId;
            ItemId = itemId;
            AuthorType = authorType;
        }
    }
}