using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.BrandPage.ValueObject;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage.Events
{
	public class BrandPageCreated : DomainEvent
	{
        public AggregateId BrandId { get; private set; }
		public List<AggregateId> SectionIds { get; private set; }
        public PageTitle Title { get; private set; }
        public ImageId CardImgId { get; private set; }
        public ImageId HeaderImgId { get; private set; }
		public UITemplateIdentifier UiTemplateIdentifier { get; private set; }
		public bool Publish { get; private set; }

		public BrandPageCreated(Guid id, AggregateId brandId, List<AggregateId> sectionIds, PageTitle title, ImageId cardImgId, ImageId headerImgId, UITemplateIdentifier uiTemplateIdentifier, bool publish) : base(id)
		{
			BrandId = brandId;
			Title = title;
			SectionIds = sectionIds;
			CardImgId = cardImgId;
			HeaderImgId = headerImgId;
			UiTemplateIdentifier = uiTemplateIdentifier;
			Publish = publish;

		}
	}
}
