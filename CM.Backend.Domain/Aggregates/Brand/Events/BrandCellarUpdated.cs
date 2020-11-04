using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Events
{
	public class BrandCellarUpdated : DomainEvent
	{
		public ImageId CardImageId { get; private set; }
		public ImageId CoverImageId { get; private set; }

		public BrandCellarUpdated(Guid id, ImageId cardImageId, ImageId coverImageId) : base(id)
		{
			CoverImageId = coverImageId;
			CardImageId = cardImageId;
		}
	}
}
