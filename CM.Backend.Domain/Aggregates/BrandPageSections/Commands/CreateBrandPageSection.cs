using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections.Commands
{
    public class CreateBrandPageSection
    {

		public AggregateId Id { get; private set; }
		public AggregateId BrandId { get; private set; }
		public string Title { get; private set; }
	    public string SubTitle { get; private set; }
	    public string Body { get; private set; }
		public List<AggregateId> ChampagneIds { get; private set; }
		public List<AggregateId> ImageIds { get; private set; }
		public bool IsDeleted { get; private set; }
        
		public CreateBrandPageSection(AggregateId id, AggregateId brandId, string title, string subTitle, string body, List<Guid> champagneIds, List<Guid> imageIds)
        {
	        if (id == null || brandId == null)
	        {
		        throw new ArgumentException(nameof(CreateBrandPageSection) + ": Paramter Id and brandId cannot be null or empty");
	        }
	        
			Id = id;
			BrandId = brandId;
			Title = title;
	        SubTitle = subTitle;
	        Body = body;
	        ChampagneIds = new List<AggregateId>(champagneIds.ConverToAggregateIds());
	        ImageIds = new List<AggregateId>(imageIds.ConverToAggregateIds());
			IsDeleted = false;
        }
    }
}
