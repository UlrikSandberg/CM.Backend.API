using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateUserEmail
    {
        public Email Email { get; private set; }

        public UpdateUserEmail(Email email)
        {
            if (email == null)
            {
                throw new ArgumentException(nameof(email));
            }
            
            Email = email;
        }
    }
}