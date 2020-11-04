using System;

namespace CM.Backend.Commands.Commands
{
    public class CreateBrand : CommandWithIdResponse
    {
        public string Name { get; private set; }
        public string ProfileText { get; private set; }
        public string FacebookUrl { get; private set; }
        public string InstagramUrl { get; private set; }
        public string PinterestUrl { get; private set; }
        public string TwitterUrl { get; private set; }
        public string WebsiteUrl { get; private set; }

        public CreateBrand(string name, string profileText, string facebookUrl, string instagramUrl, string pinterestUrl, string twitterUrl, string websiteUrl)
        {
            Name = name;
            ProfileText = profileText;
            FacebookUrl = facebookUrl;
            InstagramUrl = instagramUrl;
            PinterestUrl = pinterestUrl;
            TwitterUrl = twitterUrl;
            WebsiteUrl = websiteUrl;
        }
    }
}