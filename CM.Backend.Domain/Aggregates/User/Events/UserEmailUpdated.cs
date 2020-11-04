using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class UserEmailUpdated : DomainEvent
    {
        public Email Email { get; private set; }
        public Name Name { get; private set; }

        public UserEmailUpdated(Guid id, Email email, Name name) : base(id)
        {
            Email = email;
            Name = name;
        }
    }
}