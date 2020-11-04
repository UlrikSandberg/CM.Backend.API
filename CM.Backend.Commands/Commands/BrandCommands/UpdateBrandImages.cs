using System;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class UpdateBrandImages : Command
    {
        public Guid BrandId { get; private set; }
        public Guid BrandCoverImageId { get; private set; }
        public Guid BrandListImageId { get; private set; }
        public Guid BottleCoverImageId { get; private set; }
        public Guid LogoImageId { get; private set; }

        public UpdateBrandImages(Guid brandId, Guid brandCoverImageId, Guid brandListImageId, Guid bottleCoverImageId, Guid logoImageId)
        {
            BrandId = brandId;
            BrandCoverImageId = brandCoverImageId;
            BrandListImageId = brandListImageId;
            BottleCoverImageId = bottleCoverImageId;
            LogoImageId = logoImageId;
        }
    }
}