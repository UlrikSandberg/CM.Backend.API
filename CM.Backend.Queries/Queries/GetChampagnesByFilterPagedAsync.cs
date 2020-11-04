using System.Collections.Generic;
using System.Runtime.InteropServices;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Queries.Builders;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetChampagnesByFilterPagedAsync : Query<IEnumerable<ChampagneLight>>
    {
        public FilterSearchQuery FilterSearchQuery { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public GetChampagnesByFilterPagedAsync(FilterSearchQuery filterSearchQuery, int page, int pageSize)
        {
            FilterSearchQuery = filterSearchQuery;
            Page = page;
            PageSize = pageSize;
        }
    }
}