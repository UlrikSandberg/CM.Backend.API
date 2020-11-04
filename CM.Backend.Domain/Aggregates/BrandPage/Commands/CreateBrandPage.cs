using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.BrandPage.ValueObject;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Commands
{
    public class CreateBrandPage
    {      
		public AggregateId Id { get; private set; }
		public AggregateId BrandId { get; private set; }
		public List<AggregateId> SectionIds { get; private set; }
		public PageTitle Title { get; private set; }
		public ImageId CardImgId { get; private set; }
		public ImageId HeaderImgId { get; private set; }
	    public UITemplateIdentifier UiTemplateIdentifier { get; private set; }
	    public bool Publish { get; private set; }

		public CreateBrandPage(AggregateId id, AggregateId brandId, PageTitle title, ImageId cardImgId, ImageId headerImgId, UITemplateIdentifier uiTemplateIdentifier)
        {
	        if (id == null || brandId == null || title == null || cardImgId == null || headerImgId == null ||
	            uiTemplateIdentifier == null)
	        {
		        throw new ArgumentException(nameof(CreateBrandPage) + ": Parameter values must not be null");
	        }
	        
			Id = id;
			BrandId = brandId;
			SectionIds = new List<AggregateId>();
			Title = title;
			CardImgId = cardImgId;
			HeaderImgId = headerImgId;
	        UiTemplateIdentifier = uiTemplateIdentifier;
	        Publish = false;         
        }
    }
}
