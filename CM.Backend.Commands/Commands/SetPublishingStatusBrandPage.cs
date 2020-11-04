using System;
namespace CM.Backend.Commands.Commands
{
	public class SetPublishingStatusBrandPage : CommandWithIdResponse
    {
		public Guid BrandId { get; private set; }
		public Guid BrandPageId { get; private set; }
		public bool Publish { get; private set; }

		public SetPublishingStatusBrandPage(Guid brandId, Guid brandPageId, bool publish)
        {
			BrandId = brandId;
			BrandPageId = brandPageId;
			Publish = publish;
        }
    }
}
