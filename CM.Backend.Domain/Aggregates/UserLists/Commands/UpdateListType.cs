using System;
using CM.Backend.Domain.Aggregates.UserLists.ValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class UpdateListType
    {
        public UserListType UserListType { get; }

        public UpdateListType(UserListType userListType)
        {
            if (userListType == null)
            {
                throw new ArgumentException($"One or more values in {nameof(UpdateListType)} constructor is null");
            }
            
            UserListType = userListType;
        }
    }
}