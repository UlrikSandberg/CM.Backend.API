using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class UserInfoEdited : DomainEvent
    {
        public Name Name { get; private set; }
		public string ProfileName { get; private set; }
		public string Biography { get; private set; }
	    public bool DidNameChange { get; private set; }

	    public UserInfoEdited(Guid id, Name name, string profileName, string biography, bool didNameChange) : base(id)
        {
            Biography = biography;
	        DidNameChange = didNameChange;
	        ProfileName = profileName;
			Name = name;
		}
    }
}
