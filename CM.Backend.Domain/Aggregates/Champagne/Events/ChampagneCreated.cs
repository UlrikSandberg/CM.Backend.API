using System;
using CM.Backend.Documents.Events;

using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Events
{
	public class ChampagneCreated : DomainEvent
	{
		public BottleName BottleName { get; private set; }
		public AggregateId BrandId { get; private set; }
		public ImageId BottleImgId { get; private set; }
		public VintageInfo VintageInfo { get; private set; }
		public bool IsPublished { get; private set; }

		public ChampagneCreated(Guid champagneId, BottleName bottleName, AggregateId brandId, ImageId bottleImgId, VintageInfo vintageInfo, bool isPublished) : base(champagneId)
		{
			IsPublished = isPublished;
			VintageInfo = vintageInfo;
			BottleImgId = bottleImgId;
			BrandId = brandId;
			BottleName = bottleName;
		}
	}
}
