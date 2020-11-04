using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.ValueObjects
{
    public class UserListAuthorType : SingleValueObject<string>
    {
        public const string User = "User";
        public const string ChampagneMoments = "ChampagneMoments";

        private HashSet<string> _eligibleAuthorTypes = new HashSet<string> {User, ChampagneMoments};

        public UserListAuthorType(string authorType) : base(authorType)
        {
            if (!_eligibleAuthorTypes.Contains(authorType))
            {
                throw new ArgumentException($"UserListContentType: {authorType} is not a valid UserListContentType. --> Valid input includes: {User}, {ChampagneMoments}");
            }
        }
    }
}