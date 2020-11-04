using System;
namespace CM.Backend.Commands.Commands
{
	public class EditBrandPage : CommandWithIdResponse
    {
		public Guid PageId { get; private set; }
		public string Title { get; private set; }
		public Guid CardImgId { get; private set; }
		public Guid HeaderImgId { get; private set; }
	    public string UiTemplateIdentifier { get; private set; }

	    public EditBrandPage(Guid pageId, string title, Guid cardImgId, Guid headerImgId, string uiTemplateIdentifier)
        {
			PageId = pageId;
			HeaderImgId = headerImgId;
	        UiTemplateIdentifier = uiTemplateIdentifier;
	        CardImgId = cardImgId;
			Title = title;
		}
    }
}
