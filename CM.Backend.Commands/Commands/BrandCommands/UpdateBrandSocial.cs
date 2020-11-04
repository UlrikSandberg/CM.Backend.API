using System;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class UpdateBrandSocial : Command
    {
        public Guid BrandId { get; private set; }
        public string FacebookUrl { get; private set; }
        public string InstagramUrl { get; private set; }
        public string TwitterUrl { get; private set; }
        public string PinterestUrl { get; private set; }
        public string WebsiteUrl { get; private set; }

        public UpdateBrandSocial(Guid brandId, string facebookUrl, string instagramUrl, string twitterUrl, string pinterestUrl, string websiteUrl)
        {
            BrandId = brandId;
            FacebookUrl = facebookUrl;
            InstagramUrl = instagramUrl;
            TwitterUrl = twitterUrl;
            PinterestUrl = pinterestUrl;
            WebsiteUrl = websiteUrl;
        }
    }
}