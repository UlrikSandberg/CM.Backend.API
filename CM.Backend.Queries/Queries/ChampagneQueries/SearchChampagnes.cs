using System.Collections.Generic;
using CM.Backend.Queries.Model.ChampagneModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class SearchChampagnes : Query<IEnumerable<ChampagneSearchModel>>
    {
        public string SearchText { get; }
        public int Page { get; }
        public int PageSize { get; }

        public SearchChampagnes(string searchText, int page, int pageSize)
        {
            SearchText = searchText;
            Page = page;
            PageSize = pageSize;
        }
    }
}