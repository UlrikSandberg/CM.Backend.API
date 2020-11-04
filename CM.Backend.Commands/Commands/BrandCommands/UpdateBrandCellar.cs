using System;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class UpdateBrandCellar : Command
    {
        public Guid BrandId { get; private set; }
        public Guid CardImageId { get; private set; }
        public Guid CoverImageId { get; private set; }

        public UpdateBrandCellar(Guid brandId, Guid cardImageId, Guid coverImageId)
        {
            BrandId = brandId;
            CardImageId = cardImageId;
            CoverImageId = coverImageId;
        }
    }
}