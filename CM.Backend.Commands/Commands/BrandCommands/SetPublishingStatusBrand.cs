using System;
namespace CM.Backend.Commands.Commands
{
	public class SetPublishingStatusBrand : Command
    {
		public Guid BrandId { get; private set; }
		public bool PublishStatus{ get; private set; }

		public SetPublishingStatusBrand(Guid brandId, bool publishStatus)
        {
			BrandId = brandId;
			PublishStatus = publishStatus;
        }
    }
}
