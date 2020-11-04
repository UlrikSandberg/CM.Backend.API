using System;
using System.Collections.Generic;

namespace CM.Backend.API.RequestModels.BrandRequestModels
{
    public class UpdateCellarSectionRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Guid> Champagnes { get; set; }
    }
}