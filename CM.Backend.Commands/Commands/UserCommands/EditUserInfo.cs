using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class EditUserInfo : Command
    {
        public string Name { get; private set; }
		public string ProfileName { get; private set; }
		public string Biography { get; private set; }
		public Guid Id { get; private set; }

		public EditUserInfo(Guid id, string name, string profileName, string biography)
        {
            Id = id;
			Biography = biography;
			ProfileName = profileName;
			Name = name;
		}
    }
}
