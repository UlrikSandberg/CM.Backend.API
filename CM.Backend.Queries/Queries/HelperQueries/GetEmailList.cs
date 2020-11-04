using System;
using System.Collections;
using System.Collections.Generic;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.HelperQueries
{
    public class GetEmailList : Query<IEnumerable<string>>
    {
        public int Page { get; }
        public int PageSize { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public GetEmailList(int page, int pageSize, DateTime fromDate, DateTime toDate)
        {
            Page = page;
            PageSize = pageSize;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}