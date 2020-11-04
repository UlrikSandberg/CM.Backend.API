using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Brand.Entities;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
    public class BrandCreated : DomainEvent
    {
        public BrandName Name { get; private set; }
        public string ProfileText { get; private set; }
        public ImageId BrandCoverImageId { get; private set; }
        public ImageId BrandListImageId { get; private set; }
        public ImageId BottleCoverImageId { get; private set; }
        public ImageId LogoImageId { get; private set; }
        public InstagramUrl InstagramUrl { get; private set; }
        public FacebookUrl FacebookUrl { get; private set; }
        public PinterestUrl PinterestUrl { get; private set; }
        public TwitterUrl TwitterUrl { get; private set; }
        public UrlValueObject WebsiteUrl { get; private set; }
        public Cellar Cellar { get; private set; }
        public bool IsPublished { get; private set; }

        public BrandCreated(Guid id, BrandName name, string profileText, ImageId brandCoverImageId, ImageId brandListImageId, ImageId bottleCoverImageId, ImageId logoImageId, InstagramUrl instagramUrl, FacebookUrl facebookUrl, PinterestUrl pinterestUrl, TwitterUrl twitterUrl, UrlValueObject websiteUrl, Cellar cellar, bool isPublished) : base(id)
        {
            Name = name;
            ProfileText = profileText;
            BrandCoverImageId = brandCoverImageId;
            BrandListImageId = brandListImageId;
            BottleCoverImageId = bottleCoverImageId;
            LogoImageId = logoImageId;
            InstagramUrl = instagramUrl;
            FacebookUrl = facebookUrl;
            PinterestUrl = pinterestUrl;
            TwitterUrl = twitterUrl;
            WebsiteUrl = websiteUrl;
            Cellar = cellar;
            IsPublished = isPublished;
        }
    }
}