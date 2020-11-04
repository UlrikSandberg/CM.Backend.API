using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class SetPublishingStatusBrand
    {
		public AggregateId BrandId { get; private set; }
		public bool PublishStatus { get; private set; }

		public SetPublishingStatusBrand(AggregateId brandId, bool publishStatus)
        {
	        if (brandId == null)
	        {
		        throw new ArgumentException(nameof(brandId) + ": SetPublishingStatusBrand(AggregateId brandId,...) --> brandId is null which is not allowed");
	        }
	        
			BrandId = brandId;
			PublishStatus = publishStatus;
        }
    }
}
