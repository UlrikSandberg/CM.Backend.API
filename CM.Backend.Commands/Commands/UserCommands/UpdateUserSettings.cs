using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class UpdateUserSettings : Command
    {
        public Guid UserId { get; private set; }

        public UpdateUserSettings(Guid userId)
        {
            UserId = userId;
        }

        public string Name { get; set; }
        public string ProfileName { get; set; }
        public string Biography { get; set; }
        
        public Guid ProfileImageId { get; set; }
        public Guid ProfileCoverImageId { get; set; }
        public Guid ProfileCellarCardImageId { get; set; } 
    }
}