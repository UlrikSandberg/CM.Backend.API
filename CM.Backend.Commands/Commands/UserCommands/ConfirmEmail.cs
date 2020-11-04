using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class ConfirmEmail : Command
    {
        public Guid UserId { get; private set; }
        public string Email { get; private set; }

        public ConfirmEmail(Guid userId, string email)
        {
            UserId = userId;
            Email = email;
        }
    }
}