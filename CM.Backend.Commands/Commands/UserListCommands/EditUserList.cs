using System;
using CM.Backend.Commands.Commands.UserListCommands.Helpers;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class EditUserList : Command
    {
        public Guid AuthorId { get; }
        public Guid ListId { get; }
        public string Title { get; }
        public string Subtitle { get; }
        public string Description { get; }
        public Guid ImageId { get; }
        public string AuthorType { get; }

        public EditUserList(Guid authorId, Guid listId, string title, string subtitle, string description, Guid imageId, string authorType = DefaultKeys.DefaultUserListAuthorType)
        {
            AuthorId = authorId;
            ListId = listId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
            ImageId = imageId;
            AuthorType = authorType;
        }
    }
}