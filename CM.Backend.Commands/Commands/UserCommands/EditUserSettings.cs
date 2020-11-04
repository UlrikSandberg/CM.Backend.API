using System;
namespace CM.Backend.Commands.Commands.UserCommands
{
	public class EditUserSettings : Command
    {
        public string CountryCode { get; private set; }
		public string Language { get; private set; }
		public Guid Id { get; private set; }

		public EditUserSettings(Guid id, string countryCode, string language)
        {
            Id = id;
			Language = language;
			CountryCode = countryCode;
		}
    }
}
