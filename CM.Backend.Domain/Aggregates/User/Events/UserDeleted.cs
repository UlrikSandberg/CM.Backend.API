using System;
using CM.Backend.Documents.Events;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class UserDeleted : DomainEvent
    {
        public UserDeleted(Guid id) : base(id)
        {
        }
    }
}