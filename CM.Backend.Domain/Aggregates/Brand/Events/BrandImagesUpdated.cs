using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
    public class BrandImagesUpdated : DomainEvent
    {
        public ImageId BrandCoverImageId { get; private set; }
        public ImageId BrandListImageId { get; private set; }
        public ImageId BottleCoverImageId { get; private set; }
        public ImageId LogoImageId { get; private set; }
        public bool DidLogoImageIdChange { get; private set; }

        public BrandImagesUpdated(Guid id, ImageId brandCoverImageId, ImageId brandListImageId, ImageId bottleCoverImageId,
            ImageId logoImageId, bool didLogoImageIdChange) : base(id)
        {
            BrandCoverImageId = brandCoverImageId;
            BrandListImageId = brandListImageId;
            BottleCoverImageId = bottleCoverImageId;
            LogoImageId = logoImageId;
            DidLogoImageIdChange = didLogoImageIdChange;
        }
    }
}