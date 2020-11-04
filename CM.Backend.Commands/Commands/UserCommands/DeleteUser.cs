using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class DeleteUser : Command
    {
        public Guid UserId { get; private set; }

        public DeleteUser(Guid userId)
        {
            UserId = userId;
        }
    }
}