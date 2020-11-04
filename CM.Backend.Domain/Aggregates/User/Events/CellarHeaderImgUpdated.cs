using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
	public class CellarHeaderImgUpdated : DomainEvent
    {
		public ImageId CellarHeaderImgId { get; set; }

		public CellarHeaderImgUpdated(Guid id, ImageId cellarHeaderImgId) : base(id)
        {
            CellarHeaderImgId = cellarHeaderImgId;
		}
    }
}
