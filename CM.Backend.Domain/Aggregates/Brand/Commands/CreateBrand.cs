using System;
using CM.Backend.Domain.Aggregates.Brand.Entities;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class CreateBrand
    {
        public AggregateId Id { get; private set; }
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
        
		public CreateBrand(AggregateId id, BrandName name, string profileText, ImageId brandCoverImageId, ImageId brandListImageId, ImageId bottleCoverImageId, ImageId logoImageId, InstagramUrl instagramUrl, FacebookUrl facebookUrl, PinterestUrl pinterestUrl, TwitterUrl twitterUrl, UrlValueObject websiteUrl, Cellar cellar)
        {
            if (id == null || name == null || profileText == null || brandCoverImageId == null ||
                brandListImageId == null || bottleCoverImageId == null || logoImageId == null || instagramUrl == null ||
                facebookUrl == null || pinterestUrl == null || twitterUrl == null || websiteUrl == null ||
                cellar == null)
            {
                throw new ArgumentException(nameof(CreateBrand) + ": one or more of the constructor values are null, which is not allowed");
            }
            
            if (cellar == null)
            {
                throw new ArgumentException(nameof(cellar) + ": While creating a brand a cellar cannot be null");
            }
            
            Id = id;
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
            IsPublished = false;
        }
    }
}