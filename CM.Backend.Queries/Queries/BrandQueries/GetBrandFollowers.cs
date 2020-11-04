using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.BrandQueries
{
    public class GetBrandFollowers : Query<IEnumerable<FollowersQueryModel>>
    {
        public Guid RequesterId { get; private set; }
        public Guid BrandId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public GetBrandFollowers(Guid requesterId, Guid brandId, int page, int pageSize)
        {
            RequesterId = requesterId;
            BrandId = brandId;
            Page = page;
            PageSize = pageSize;
        }
        
        
    }
}