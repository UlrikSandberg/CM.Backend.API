namespace CM.Backend.Persistence.Model.Entities
{
    public class UserSettings
    {
        public string CountryCode { get; set; }
        public string Language { get; set; }
			
        public bool IsEmailVerified { get; set; }
    }
}