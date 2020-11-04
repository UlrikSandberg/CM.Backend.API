using System;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;

namespace CM.Backend.Queries.Model
{
	   
	public class CellarSection
	{
	    public Guid SectionId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
		public List<Guid> Champagnes { get; set; } = new List<Guid>();
	}
}
