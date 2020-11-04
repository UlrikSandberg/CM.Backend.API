using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Commands
{
    public class RemoveSection
    {
		public AggregateId BrandId { get; private set; }
		public AggregateId SectionId { get; private set; }

		public RemoveSection(AggregateId brandId, AggregateId sectionId)
        {
	        if (brandId == null || sectionId == null)
	        {
				throw new ArgumentException(nameof(RemoveSection) + ": Some parameter values are null which is not eligible");
	        }
	        
            SectionId = sectionId;
			BrandId = brandId;
		}
    }
}
