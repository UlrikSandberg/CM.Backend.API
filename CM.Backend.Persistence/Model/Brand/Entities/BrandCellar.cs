using System;
using System.Collections.Generic;

namespace CM.Backend.Persistence.Model.Entities
{
    public class BrandCellar
    {
        public string Title { get; set; }
        public Guid CardImgId { get; set; }
        public Guid CoverImgId { get; set; }
        public string Url { get; set; }
        public List<CellarSection> Sections { get; set; } = new List<CellarSection>();
    }
}