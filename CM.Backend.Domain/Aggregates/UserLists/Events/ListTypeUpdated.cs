using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.UserLists.ValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class ListTypeUpdated : DomainEvent
    {
        public UserListType UserListType { get; }

        public ListTypeUpdated(Guid id, UserListType userListType) : base(id)
        {
            UserListType = userListType;
        }
    }
}