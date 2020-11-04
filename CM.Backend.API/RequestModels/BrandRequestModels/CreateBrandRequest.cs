using System;

namespace CM.Backend.API.RequestModels
{
    public class CreateBrandRequest
    {
        public string Name { get; set; }
        public string ProfileText { get; set; }
        public string FacebookUrl { get; set; }
        public string InstagramUrl { get; set; }
        public string PinterestUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string WebsiteUrl { get; set; }
    }
}