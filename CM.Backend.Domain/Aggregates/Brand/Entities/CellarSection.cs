using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Entities
{
    public class CellarSection
    {
	    public AggregateId Id { get; private set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public List<AggregateId> Champagnes { get; set; }

        public CellarSection(AggregateId id, string title, string body, List<AggregateId> champagnes)
        {
	        if (champagnes == null)
	        {
		        throw new ArgumentException(nameof(champagnes) + ": CellarSection -> List<AggregateId> champagnes cannot be null");
	        }
	        
	        Id = id;
	        Title = title;
			Body = body;
			Champagnes = champagnes;
        }
    }
}
