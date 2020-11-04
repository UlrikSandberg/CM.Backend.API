using System;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class DeleteBrandCellarSection : Command
    {
        public Guid BrandId { get; private set; }
        public Guid SectionId { get; private set; }

        public DeleteBrandCellarSection(Guid brandId, Guid sectionId)
        {
            BrandId = brandId;
            SectionId = sectionId;
        }
    }
}