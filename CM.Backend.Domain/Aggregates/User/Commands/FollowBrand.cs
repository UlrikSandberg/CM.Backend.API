using System;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class FollowBrand
    {
        public AggregateId BrandId { get; private set; }
		public BrandName BrandName { get; private set; }
		public ImageId BrandLogoId { get; private set; }

		public FollowBrand(AggregateId brandId, BrandName brandName, ImageId brandLogoId)
        {
	        if (brandId == null || brandName == null || brandLogoId == null)
	        {
		        throw new ArgumentException(nameof(FollowBrand) + ": Paramter values may not be null");
	        }
	        
            BrandId = brandId;
			BrandName = brandName;
			BrandLogoId = brandLogoId;
		}
    }
}
