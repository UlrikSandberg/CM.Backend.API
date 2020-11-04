using System;
using System.Collections.Generic;
using CM.Backend.Queries.Queries.HelperQueries.HelperModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.HelperQueries
{
    public class GetEmailListVerbose : Query<IEnumerable<VerboseEmailListModel>>
    {
        public int Page { get; }
        public int PageSize { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public GetEmailListVerbose(int page, int pageSize, DateTime fromDate, DateTime toDate)
        {
            Page = page;
            PageSize = pageSize;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}