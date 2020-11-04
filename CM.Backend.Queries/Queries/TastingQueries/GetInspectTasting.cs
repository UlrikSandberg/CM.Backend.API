using System;
using CM.Backend.Queries.Model.TastingModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.TastingQueries
{
    public class GetInspectTasting : Query<InspectTastingModel>
    {
        public Guid RequesterId { get; private set; }
        public Guid TastingId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public bool OrderByAcendingDate { get; private set; }

        public GetInspectTasting(Guid requesterId, Guid tastingId, int page, int pageSize,
            bool orderByAcendingDate = false)
        {
            RequesterId = requesterId;
            TastingId = tastingId;
            Page = page;
            PageSize = pageSize;
            OrderByAcendingDate = orderByAcendingDate;
        }
    }
}