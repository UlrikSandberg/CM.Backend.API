using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections.Commands
{
    public class DeleteBrandPageSection
    {
		public AggregateId BrandId { get; private set; }
		public AggregateId BrandPageId { get; private set; }
		public AggregateId SectionId { get; private set; }

		public DeleteBrandPageSection(AggregateId brandId, AggregateId brandPageId, AggregateId sectionId)
        {
	        if (brandId == null || brandPageId == null || sectionId == null)
	        {
				throw new ArgumentException(nameof(DeleteBrandPageSection) + ": Parameter values must not be null");    
	        }
	        
			BrandId = brandId;
			BrandPageId = brandPageId;
			SectionId = sectionId;
        }
    }
}
