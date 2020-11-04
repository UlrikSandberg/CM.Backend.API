using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.ValueObjects
{
    public class UserListType : SingleValueObject<string>
    {
        public const string Top10List = "Top10List";
        public const string InfiniteList = "InfiniteList";

        private HashSet<string> _eligibleListTypes = new HashSet<string> {Top10List, InfiniteList};
        
        public UserListType(string listType) : base(listType)
        {
            if (!_eligibleListTypes.Contains(listType))
            {
                throw new ArgumentException($"Input listType: {listType} is not a valid listType. --> Valid listTypes includes: {Top10List}, {InfiniteList}");
            }
        }
    }
}