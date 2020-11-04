using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class ProfileCoverUpdated : DomainEvent
    {
		public ImageId ProfileCoverImgId { get; private set; }

		public ProfileCoverUpdated(Guid id, ImageId profileCoverImgId) : base(id)
        {
            ProfileCoverImgId = profileCoverImgId;
		}
    }
}
