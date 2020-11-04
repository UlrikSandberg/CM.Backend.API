using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model.CommentModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.TastingQueries
{
    public class GetComments : Query<IEnumerable<CommentModel>>
    {
        public Guid RequesterId { get; private set; }
        public Guid ContextId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public bool AcendingOrderByDate { get; private set; }

        public GetComments(Guid requesterId, Guid contextId, int page, int pageSize, bool acendingOrderByDate)
        {
            RequesterId = requesterId;
            ContextId = contextId;
            Page = page;
            PageSize = pageSize;
            AcendingOrderByDate = acendingOrderByDate;
        }
        
    }
}