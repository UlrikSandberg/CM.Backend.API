using System;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Commands
{
    public class EditChampagne
    {
		public BottleName BottleName { get; private set; }
		public ImageId BottleImgId { get; private set; }
		public VintageInfo VintageInfo { get; private set; }

		public EditChampagne(BottleName bottleName, ImageId bottleImgId, VintageInfo vintageInfo)
        {
	        if (bottleName == null || bottleImgId == null || vintageInfo == null)
	        {
		        throw new ArgumentException(nameof(EditChampagne) + ": Parameter values may not be null");
	        }
	        
            VintageInfo = vintageInfo;
			BottleImgId = bottleImgId;
			BottleName = bottleName;
		}
    }
}
