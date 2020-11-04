using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Commands
{
    public class AddSection
    {
		public AggregateId BrandId { get; private set; }
		public AggregateId SectionId { get; private set; }

		public AddSection(AggregateId brandId, AggregateId sectionId)
        {
	        if (brandId == null || sectionId == null)
	        {
		        throw new ArgumentException(nameof(AddSection) + ": Parameter value must not be null");
	        }
	        
			BrandId = brandId;
			SectionId = sectionId;
		}
    }
}
