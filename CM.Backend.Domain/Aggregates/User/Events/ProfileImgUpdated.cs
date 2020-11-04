using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class ProfileImgUpdated : DomainEvent
    {
		public ImageId ProfileImgId { get; private set; }

		public ProfileImgUpdated(Guid id, ImageId profileImgId) : base(id)
        {
            ProfileImgId = profileImgId;
		}
    }
}
