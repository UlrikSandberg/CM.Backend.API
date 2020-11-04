using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetBrand : Query<BrandProfileExtendedBrandPage>
    {
        public Guid RequesterId { get; private set; }
        public Guid BrandId { get; private set; }

        public GetBrand(Guid brandId, Guid requesterId)
        {
            RequesterId = requesterId;
            BrandId = brandId;
        }
    }
}