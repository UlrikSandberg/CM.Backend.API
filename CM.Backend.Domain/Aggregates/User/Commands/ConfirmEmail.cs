using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class ConfirmEmail
    {
        public Email Email { get; private set; }
        public bool IsEmailVerified { get; private set; }

        public ConfirmEmail(Email email, bool isEmailVerified)
        {
            if (email == null)
            {
                throw new ArgumentException(nameof(email));
            }
            
            Email = email;
            IsEmailVerified = isEmailVerified;
        }
    }
}