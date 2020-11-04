using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.HelperQueries
{
    public class GetSubmittedFeedbackQuery : Query<IEnumerable<BugAndFeedback>>
    {
        public int Page { get; }
        public int PageSize { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }

        public GetSubmittedFeedbackQuery(int page, int pageSize, DateTime fromDate, DateTime toDate)
        {
            Page = page;
            PageSize = pageSize;
            FromDate = fromDate;
            ToDate = toDate;
        }
    }
}