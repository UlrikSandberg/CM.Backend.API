using System;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class UpdateSocial
    {
        public InstagramUrl InstagramUrl { get; private set; }
        public FacebookUrl FacebookUrl { get; private set; }
        public PinterestUrl PinterestUrl { get; private set; }
        public TwitterUrl TwitterUrl { get; private set; }
        public UrlValueObject WebsiteUrl { get; private set; }

        public UpdateSocial(InstagramUrl instagramUrl, FacebookUrl facebookUrl, PinterestUrl pinterestUrl, TwitterUrl twitterUrl, UrlValueObject websiteUrl)
        {
            if (instagramUrl == null || facebookUrl == null || pinterestUrl == null || twitterUrl == null ||
                websiteUrl == null)
            {
                throw new ArgumentException(nameof(UpdateSocial) + ": UpdateSocial(...) --> One or more of the constructor parameters are null which is not given in the current context");
            }
            
            InstagramUrl = instagramUrl;
            FacebookUrl = facebookUrl;
            PinterestUrl = pinterestUrl;
            TwitterUrl = twitterUrl;
            WebsiteUrl = websiteUrl;
        }
    }
}