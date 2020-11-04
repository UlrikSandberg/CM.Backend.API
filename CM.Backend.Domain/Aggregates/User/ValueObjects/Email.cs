using System;
using CM.Backend.Domain.SharedValueObjects;
using EmailValidation;

namespace CM.Backend.Domain.Aggregates.User.ValueObjects
{
    public class Email : SingleValueObject<string>
    {
        public Email(string value) : base(value)
        {
            if (!EmailValidator.Validate(value))
            {
                throw new ArgumentException(nameof(value) + "Invalid email");
            }
        }
    }
}