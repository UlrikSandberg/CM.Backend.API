using System;
namespace CM.Backend.Commands.Commands
{
	public class SetChampagnePublishingStatus : Command
    {
		public Guid BrandId { get; private set; }
		public Guid ChampagneId { get; set; }
		public bool IsPublished { get; private set; }

		public SetChampagnePublishingStatus(Guid brandId, Guid champagneId, bool IsPublished)
        {
            this.IsPublished = IsPublished;
			ChampagneId = champagneId;
			BrandId = brandId;
		}
    }
}
