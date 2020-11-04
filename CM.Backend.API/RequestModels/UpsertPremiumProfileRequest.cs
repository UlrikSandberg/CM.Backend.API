namespace CM.Backend.API.RequestModels
{
    public class UpsertPremiumProfileRequest
    {
        public string FacebookUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string PinterestUrl { get; set; }
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string TelNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }
        public string LogoImageId { get; set; }
        public string ContactImageId { get; set; }
    }
}