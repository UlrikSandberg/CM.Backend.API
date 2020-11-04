using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections.Commands
{
    public class EditSection
    {
		public string Title { get; private set; }
	    public string SubTitle { get; private set; }
	    public string Body { get; private set; }
		public List<AggregateId> ImageIds { get; private set; }
		public List<AggregateId> ChampagneIds { get; private set; }
        
		public EditSection(string title, string subTitle, string body, List<Guid> imageIds, List<Guid> champagneIds)
        {
	        
			ChampagneIds = new List<AggregateId>(champagneIds.ConverToAggregateIds());
			ImageIds = new List<AggregateId>(imageIds.ConverToAggregateIds());
			Body = body;
			Title = title;
	        SubTitle = subTitle;
        }
    }
}
