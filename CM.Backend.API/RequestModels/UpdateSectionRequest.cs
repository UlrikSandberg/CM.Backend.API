using System.Collections.Generic;

namespace CM.Backend.API.RequestModels
{
    public class UpdateSectionRequest
    {
        public string HeaderText { get; set; }
        public string BodyText { get; set; }
        public IEnumerable<string> ImageIds { get; set; }
    }
}