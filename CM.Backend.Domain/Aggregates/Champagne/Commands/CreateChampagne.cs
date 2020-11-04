using System;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Commands
{
    public class CreateChampagne
    {
		public AggregateId Id { get; private set; }
		public BottleName BottleName { get; private set; }
		public AggregateId BrandId { get; private set; }
		public ImageId BottleImgId { get; private set; }
		public VintageInfo VintageInfo { get; private set; }
		public bool IsPublished { get; private set; }

		public CreateChampagne(AggregateId id, BottleName bottleName, AggregateId brandId, ImageId bottleImgId, VintageInfo vintageInfo)
		{
			if (id == null || bottleName == null || brandId == null || bottleImgId == null || vintageInfo == null)
			{
				throw new ArgumentException(nameof(CreateChampagne) + ": Parameter values may not be null");
			}
			
			VintageInfo = vintageInfo;
			BottleImgId = bottleImgId;
			BrandId = brandId;
			BottleName = bottleName;
			Id = id;
			IsPublished = false;
		}
    }
}