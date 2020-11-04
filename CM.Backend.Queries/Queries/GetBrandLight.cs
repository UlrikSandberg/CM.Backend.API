using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetBrandLight : Query<BrandLight>
    {
        public Guid BrandId { get; private set; }

        public GetBrandLight(Guid brandId)
        {
            BrandId = brandId;
        }
    }
}
