using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;

namespace CM.Backend.Queries.Model
{
    public class BrandPageSectionModel
    {
        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public string Title { get; set; }
	    
        public string SubTitle { get; set; }

        public string Body { get; set; }

        public Guid[] Champagnes { get; set; } = new Guid[0];

        public Guid[] ImageIds { get; set; } = new Guid[0];

        public List<BrandImage> Images { get; set; } = new List<BrandImage>();
    }
}