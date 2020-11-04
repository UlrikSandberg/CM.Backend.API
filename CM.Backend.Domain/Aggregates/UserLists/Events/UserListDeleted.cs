using System;
using CM.Backend.Documents.Events;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class UserListDeleted : DomainEvent
    {
        public bool IsDeleted { get; }

        public UserListDeleted(Guid id, bool isDeleted) : base(id)
        {
            IsDeleted = isDeleted;
        }
    }
}