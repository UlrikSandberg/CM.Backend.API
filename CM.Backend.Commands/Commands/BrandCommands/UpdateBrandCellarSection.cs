using System;
using System.Collections.Generic;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class UpdateBrandCellarSection : Command
    {
        public Guid BrandId { get; private set; }
        public Guid SectionId { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public List<Guid> ChampagneIds { get; private set; }

        public UpdateBrandCellarSection(Guid brandId, Guid sectionId, string title, string body,
            List<Guid> champagneIds)
        {
            BrandId = brandId;
            SectionId = sectionId;
            Title = title;
            Body = body;
            ChampagneIds = champagneIds;
        }
    }
}