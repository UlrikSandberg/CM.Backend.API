using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class EmailConfirmed : DomainEvent
    {
        public Email Email { get; private set; }
        public bool IsEmailVerified { get; private set; }

        public EmailConfirmed(Guid id, Email email, bool isEmailVerified) : base(id)
        {
            Email = email;
            IsEmailVerified = isEmailVerified;
        }
    }
}