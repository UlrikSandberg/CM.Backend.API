using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
    public class BrandSocialUpdated : DomainEvent
    {
        public InstagramUrl InstagramUrl { get; private set; }
        public FacebookUrl FacebookUrl { get; private set; }
        public PinterestUrl PinterestUrl { get; private set; }
        public TwitterUrl TwitterUrl { get; private set; }
        public UrlValueObject WebsiteUrl { get; private set; }

        public BrandSocialUpdated(Guid id, InstagramUrl instagramUrl, FacebookUrl facebookUrl, PinterestUrl pinterestUrl, TwitterUrl twitterUrl, UrlValueObject websiteUrl) : base(id)
        {
            InstagramUrl = instagramUrl;
            FacebookUrl = facebookUrl;
            PinterestUrl = pinterestUrl;
            TwitterUrl = twitterUrl;
            WebsiteUrl = websiteUrl;
        }
    }
}