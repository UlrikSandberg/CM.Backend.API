using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class UserSettingsEdited : DomainEvent
    {
        public CountryCode CountryCode { get; private set; }
		public Language Language { get; private set; }

		public UserSettingsEdited(Guid id, CountryCode countryCode, Language language) : base(id)
        {
            Language = language;
			CountryCode = countryCode;
		}
    }
}
