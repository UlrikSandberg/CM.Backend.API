using System;
using CM.Backend.Commands.Commands.UserListCommands.Helpers;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class RemoveItem : Command
    {
        public Guid AuthorId { get; }
        public Guid ListId { get; }
        public Guid ItemId { get; }
        public string AuthorType { get; }
        
        public RemoveItem(Guid authorId, Guid listId, Guid itemId, string AuthorType = DefaultKeys.DefaultUserListAuthorType)
        {
            AuthorId = authorId;
            ListId = listId;
            ItemId = itemId;
            this.AuthorType = AuthorType;
        }
    }
}