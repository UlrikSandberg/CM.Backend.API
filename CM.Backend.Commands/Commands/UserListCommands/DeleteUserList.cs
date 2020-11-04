using System;
using System.Threading;
using CM.Backend.Commands.Commands.UserListCommands.Helpers;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class DeleteUserList : Command
    {
        public Guid AuthorId { get; }
        public Guid ListId { get; }
        public string AuthorType { get; }
        
        public DeleteUserList(Guid authorId, Guid listId, string authorType = DefaultKeys.DefaultUserListAuthorType)
        {
            AuthorId = authorId;
            ListId = listId;
            AuthorType = authorType;
        }
    }
}