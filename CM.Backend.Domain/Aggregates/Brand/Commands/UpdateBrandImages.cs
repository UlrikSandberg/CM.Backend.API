using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class UpdateBrandImages
    {
        public ImageId BrandCoverImageId { get; private set; }
        public ImageId BrandListImageId { get; private set; }
        public ImageId BottleCoverImageId { get; private set; }
        public ImageId LogoImageId { get; private set; }

        public UpdateBrandImages(ImageId brandCoverImageId, ImageId brandListImageId, ImageId bottleCoverImageId,
            ImageId logoImageId)
        {
            if (brandCoverImageId == null || brandListImageId == null || bottleCoverImageId == null ||
                logoImageId == null)
            {
                throw new ArgumentException(nameof(UpdateBrandImages) + ": UpdateBrandImages(ImageIds...) --> One ore more of the constructor parameters are null which is not allowed in the given context");
            }
            
            BrandCoverImageId = brandCoverImageId;
            BrandListImageId = brandListImageId;
            BottleCoverImageId = bottleCoverImageId;
            LogoImageId = logoImageId;
        }
    }
}
