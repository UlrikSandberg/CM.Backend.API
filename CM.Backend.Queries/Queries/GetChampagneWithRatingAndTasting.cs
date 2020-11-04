using System;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetChampagneWithRatingAndTasting : Query<ChampagneWithRatingAndTasting>
    {
        public Guid RequesterId { get; private set; }
        public Guid ChampagneId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public TastingOrderByOption.OrderBy OrderByOption { get; private set; }

        public GetChampagneWithRatingAndTasting(Guid requesterId, Guid champagneId, int page, int pageSize,
           TastingOrderByOption.OrderBy orderByOption)
        {
            RequesterId = requesterId;
            ChampagneId = champagneId;
            Page = page;
            PageSize = pageSize;
            OrderByOption = orderByOption;
        }
    }
}