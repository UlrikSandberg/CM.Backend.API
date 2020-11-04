using System.Collections.Generic;

namespace CM.Backend.API.RequestModels
{
    public class UpdateBrandPageRequest
    {
        public string HeaderImageId { get; set; }
        public string Header { get; set; }
        public IEnumerable<string> SectionIds { get; set; }
    }
}