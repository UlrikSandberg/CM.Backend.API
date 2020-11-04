using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.ValueObjects
{
    public class UserListContentType : SingleValueObject<string>
    {
        public const string ChampagneContent = "ChampagneContent";

        private HashSet<string> _eligibleListContentTypes = new HashSet<string> {ChampagneContent};
        
        public UserListContentType(string listContentType) : base(listContentType)
        {
            if (!_eligibleListContentTypes.Contains(listContentType))
            {
                throw new ArgumentException($"UserListContentType: {listContentType} is not a valid UserListContentType. --> Valid input includes: {ChampagneContent}");
            }
        }
    }
}