using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Commands
{
    public class PublishBrandPage
    {
		public AggregateId BrandId { get; private set; }
		public AggregateId BrandPageId { get; private set; }
		public bool Publish { get; private set; }

		public PublishBrandPage(AggregateId brandId, AggregateId brandPageId, bool publish)
        {
	        if (brandId == null || brandPageId == null)
	        {
		        throw new ArgumentException(nameof(PublishBrandPage) + ": Parameter values must not be null");
	        }
	        
			BrandId = brandId;
			BrandPageId = brandPageId;
			Publish = publish;
        }
    }
}
