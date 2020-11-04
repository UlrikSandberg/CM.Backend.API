using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class CellarCardImgUpdated : DomainEvent
    {
		public ImageId CellarCardImgId { get; private set; }

		public CellarCardImgUpdated(Guid id, ImageId cellarCardImgId) : base(id)
        {
            CellarCardImgId = cellarCardImgId;
		}
    }
}
