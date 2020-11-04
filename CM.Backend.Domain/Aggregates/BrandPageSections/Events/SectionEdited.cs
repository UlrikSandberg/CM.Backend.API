using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections.Events
{
	public class SectionEdited : DomainEvent
	{
		public string Title { get; set; }
		public string SubTitle { get; private set; }
		public string Body { get; set; }
		public List<AggregateId> ImageIds { get; set; }
		public List<AggregateId> ChampagneIds { get; set; }
        
		public SectionEdited(Guid id, string title, string subTitle, string body, List<AggregateId> imageIds, List<AggregateId> champagneIds) : base(id)
		{
			ChampagneIds = champagneIds;
			ImageIds = imageIds;
			Body = body;
			Title = title;
			SubTitle = subTitle;
		}
	}
}
