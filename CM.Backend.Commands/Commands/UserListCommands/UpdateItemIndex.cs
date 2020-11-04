using System;
using CM.Backend.Commands.Commands.UserListCommands.Helpers;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class UpdateItemIndex : Command
    {
        public Guid AuthorId { get; }
        public Guid ListId { get; }
        public Guid ItemId { get; }
        public int ItemIndex { get; }
        public string AuthorType { get; }

        public UpdateItemIndex(Guid authorId, Guid listId, Guid itemId, int itemIndex, string authorType = DefaultKeys.DefaultUserListAuthorType)
        {
            AuthorId = authorId;
            ListId = listId;
            ItemId = itemId;
            ItemIndex = itemIndex;
            AuthorType = authorType;
        }
    }
}