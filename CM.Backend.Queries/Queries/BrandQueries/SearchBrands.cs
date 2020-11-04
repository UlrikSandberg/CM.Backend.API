using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.BrandQueries
{
    public class SearchBrands : Query<IEnumerable<BrandSearchProjectionModel>>
    {
        public string SearchText { get; }
        public int Page { get; }
        public int PageSize { get; }

        public SearchBrands(string searchText, int page, int pageSize)
        {
            SearchText = searchText;
            Page = page;
            PageSize = pageSize;
        }
    }
}