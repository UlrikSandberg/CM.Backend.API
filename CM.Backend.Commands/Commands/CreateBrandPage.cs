using System;

namespace CM.Backend.Commands.Commands
{
	public class CreateBrandPage : CommandWithIdResponse
	{
		public Guid BrandId { get; private set; }
		public string Title { get; private set; }
		public Guid CardImgId { get; private set; }
		public Guid HeaderImgId { get; private set; }
		public string UiTemplateIdentifier { get; private set; }

		public CreateBrandPage(Guid brandId, string Title, Guid cardImgId, Guid headerImgId, string uiTemplateIdentifier)
		{
			this.HeaderImgId = headerImgId;
			UiTemplateIdentifier = uiTemplateIdentifier;
			this.CardImgId = cardImgId;
			this.Title = Title;
			this.BrandId = brandId;

		}
    }
}
