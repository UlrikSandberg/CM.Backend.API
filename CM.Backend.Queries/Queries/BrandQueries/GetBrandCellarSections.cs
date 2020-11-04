using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.BrandQueries
{
    public class GetBrandCellarSections : Query<IEnumerable<CellarSection>>
    {
        public Guid BrandId { get; private set; }

        public GetBrandCellarSections(Guid brandId)
        {
            BrandId = brandId;
        }
    }
}