using System;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.BrandQueries
{
    public class GetBrandCellarSection : Query<CellarSection>
    {
        public Guid BrandId { get; private set; }
        public Guid SectionId { get; private set; }

        public GetBrandCellarSection(Guid brandId, Guid sectionId)
        {
            BrandId = brandId;
            SectionId = sectionId;
        }
    }
}