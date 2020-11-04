using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Events
{
	public class ChampagneEdited : DomainEvent
    {
		public BottleName BottleName { get; private set; }
		public ImageId BottleImgId { get; private set; }
        public VintageInfo VintageInfo { get; set; }

		public ChampagneEdited(Guid id, BottleName bottleName, ImageId bottleImgId, VintageInfo vintageInfo) : base(id)
        {
            VintageInfo = vintageInfo;
			BottleImgId = bottleImgId;
			BottleName = bottleName;
		}
    }
}
