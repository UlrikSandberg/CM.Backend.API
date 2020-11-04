using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class EditUserInfo
    {
        public Name Name { get; set; }
		public string ProfileName { get; set; }
		public string Biography { get; set; }

		public EditUserInfo(Name name, string profileName, string biography)
        {
	        if (name == null)
	        {
		        throw new ArgumentException(nameof(name));
	        }
            Biography = biography;
			ProfileName = profileName;
			Name = name;
		}
    }
}
