using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class ResendConfirmationEmail : Command
    {
        public Guid UserId { get; private set; }

        public ResendConfirmationEmail(Guid userId)
        {
            UserId = userId;
        }
    }
}