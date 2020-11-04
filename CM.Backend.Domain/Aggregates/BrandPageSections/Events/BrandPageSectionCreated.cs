using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections.Events
{
	public class BrandPageSectionCreated : DomainEvent
	{
		public AggregateId BrandId { get; private set; }
        public string Title { get; private set; }
		public string SubTitle { get; private set; }
		public string Body { get; private set; }
        public List<AggregateId> ChampagneIds { get; private set; }
        public List<AggregateId> ImageIds { get; private set; }
		public bool IsDeleted { get; private set; }
        
		public BrandPageSectionCreated(Guid id, AggregateId brandId, string title, string subTitle, string body, List<AggregateId> champangeIds, List<AggregateId> imageIds, bool isDeleted) : base(id)
		{
			BrandId = brandId;
			Title = title;
			SubTitle = subTitle;
			Body = body;
			ChampagneIds = champangeIds;
			ImageIds = imageIds;
			IsDeleted = isDeleted;
		}
	}
}
