using System;
using System.Collections.Generic;

namespace CM.Backend.Persistence.Model.Entities
{
    public class CellarSection
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public List<Guid> Champagnes { get; set; } = new List<Guid>();
    }
}