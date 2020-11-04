using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class UpdateUserEmail : Command
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }

        public UpdateUserEmail(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}