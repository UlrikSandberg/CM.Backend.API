using System;

namespace CM.Backend.API.RequestModels.BrandRequestModels
{
    public class UpdateBrandImagesRequest
    {
        public Guid BrandCoverImageId { get; set; }
        public Guid BrandListImageId { get; set; }
        public Guid BottleCoverImageId { get; set; }
        public Guid BrandLogoImageId { get; set; }
    }
}