using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class BrandFollowed : DomainEvent
    {
		public AggregateId BrandId { get; private set; }
        public NotEmptyString FollowByName { get; set; }
		public ImageId FollowByImageId { get; set; }
		public BrandName BrandName { get; set; }
		public ImageId BrandLogoImgId { get; set; }

		public BrandFollowed(Guid id, NotEmptyString followByName, ImageId followByImageId, AggregateId brandId, BrandName brandName, ImageId brandLogoImgId) : base(id)
        {
            BrandLogoImgId = brandLogoImgId;
			BrandName = brandName;
			FollowByImageId = followByImageId;
			FollowByName = followByName;
			BrandId = brandId;

		}
    }
}
