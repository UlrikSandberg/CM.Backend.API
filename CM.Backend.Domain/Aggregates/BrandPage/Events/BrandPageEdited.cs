using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.BrandPage.ValueObject;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Events
{
	public class BrandPageEdited : DomainEvent
	{
		public PageTitle Title { get; private set; }
		public ImageId CardImgId { get; private set; }
		public ImageId HeaderImgId { get; private set; }
		public UITemplateIdentifier UiTemplateIdentifier { get; private set; }

		public BrandPageEdited(Guid id, PageTitle title, ImageId cardImgId, ImageId headerImgId, UITemplateIdentifier uiTemplateIdentifier) : base(id)
		{
			HeaderImgId = headerImgId;
			UiTemplateIdentifier = uiTemplateIdentifier;
			CardImgId = cardImgId;
			Title = title;
		}
	}
}
