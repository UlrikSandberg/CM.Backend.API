using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class EditUserSettings
    {
        public CountryCode CountryCode { get; private set; }
		public Language Language { get; private set; }

		public EditUserSettings(CountryCode countryCode, Language language)
        {
	        if (countryCode == null || language == null)
	        {
		        throw new ArgumentException(nameof(EditUserSettings) + ": Parameter values may not be null");
	        }
	        
            Language = language;
			CountryCode = countryCode;
		}
    }
}
