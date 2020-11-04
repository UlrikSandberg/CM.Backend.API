using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UnfollowBrand
    {
		public AggregateId BrandId { get; set; }

		public UnfollowBrand(AggregateId brandId)
        {
	        if (brandId == null)
	        {
		        throw new ArgumentException(nameof(brandId));
	        }
	        
            BrandId = brandId;
		}
    }
}
