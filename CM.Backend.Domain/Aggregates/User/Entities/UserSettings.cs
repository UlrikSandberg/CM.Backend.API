using CM.Backend.Domain.Aggregates.User.ValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Entities
{
    public class UserSettings
    {
        public CountryCode CountryCode { get; set; }
        public Language Language { get; set; }

        public SleepSettings SleepSettings { get; set; }
        
        public bool IsEmailVerified { get; set; }
    }
}
