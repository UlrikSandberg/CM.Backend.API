using System;
using CM.Backend.Commands.Commands.UserListCommands.Helpers;

namespace CM.Backend.Commands.Commands.UserListCommands
{
    public class CreateUserList : CommandWithIdResponse
    {
        public Guid AuthorId { get; }
        public Guid ImageId { get; }
        public string Title { get; }
        public string Subtitle { get; }
        public string Description { get; }
        public string ListType { get; }
        public string AuthorType { get; }

        public CreateUserList(Guid authorId, Guid imageId, string title, string subtitle, string description, string listType, string authorType = DefaultKeys.DefaultUserListAuthorType)
        {
            AuthorId = authorId;
            ImageId = imageId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
            ListType = listType;
            AuthorType = authorType;
        }
    }
}
