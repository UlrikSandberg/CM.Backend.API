using System;
using CM.Backend.Domain.Aggregates.BrandPage.ValueObject;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Commands
{
    public class EditBrandPage
    {
		public PageTitle Title { get; private set; }
		public ImageId CardImgId { get; private set; }
		public ImageId HeaderImgId { get; private set; }
	    public UITemplateIdentifier UiTemplateIdentifier { get; private set; }

	    public EditBrandPage(PageTitle title, ImageId cardImgId, ImageId headerImgId, UITemplateIdentifier uiTemplateIdentifier)
        {
	        if (title == null || cardImgId == null || headerImgId == null || uiTemplateIdentifier == null)
	        {
		        throw new ArgumentException(nameof(EditBrandPage) + ": Parameter values cannot be null");
	        }
	        
            HeaderImgId = headerImgId;
	        UiTemplateIdentifier = uiTemplateIdentifier;
	        CardImgId = cardImgId;
			Title = title;

		}
    }
}
