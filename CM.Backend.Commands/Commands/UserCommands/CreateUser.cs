using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
	public class CreateUser : CommandWithIdResponse
    {
        public string Email { get; private set; }
		public string Name { get; private set; }
		public string ProfileName { get; private set; }
	    public string Biography { get; private set; }
	    public long UtcOffset { get; private set; }

	    public CreateUser(string email, string name, string profileName, string biography, long utcOffset)
        {
	        Biography = biography;
	        UtcOffset = utcOffset;
	        ProfileName = profileName;
			Name = name;
			Email = email;
		}
    }
}
