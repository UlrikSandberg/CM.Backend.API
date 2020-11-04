using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CM.Backend.Domain.Aggregates.BrandPage.Commands;
using CM.Backend.Domain.Aggregates.BrandPage.Events;
using CM.Backend.Domain.Aggregates.BrandPage.ValueObject;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPage
{
	public class BrandPage : Aggregate
    {
		public AggregateId BrandId { get; private set; }
		public List<AggregateId> SectionIds { get; private set; }
		public PageTitle Title { get; private set; }
		public ImageId CardImgId { get; private set; }
		public ImageId HeaderImgId { get; private set; }
	    public UITemplateIdentifier UITemplateIdentifier { get; private set; }
		public bool Published { get; private set; }
        
		public void Execute(CreateBrandPage cmd)
		{
			RaiseEvent(new BrandPageCreated(
				cmd.Id.Value,
				cmd.BrandId,
				cmd.SectionIds,
				cmd.Title,
				cmd.CardImgId,
				cmd.HeaderImgId,
				cmd.UiTemplateIdentifier,
				cmd.Publish
                
			));         
		}

		public void Execute(PublishBrandPage cmd)
		{

			if(Id == null)
			{
				throw DomainException.CausedBy("Cannot publish non-existing brandPage");
			}
            
			RaiseEvent(new BrandPagePublished(
				Id,
				cmd.BrandId,
				cmd.BrandPageId,
				cmd.Publish
			));

		}

		public void Execute(EditBrandPage cmd)
		{

			if(Id == null)
			{
				throw DomainException.CausedBy("Cannot edit non-existing brandPage");
			}

			RaiseEvent(new BrandPageEdited(
				Id,
				cmd.Title,
				cmd.CardImgId,
				cmd.HeaderImgId,
				cmd.UiTemplateIdentifier
			));

		}

		public void Execute(AddSection cmd)
		{

			if(Id == null)
			{
				throw DomainException.CausedBy("Can't add a section to a non-existing brandPage");
			}

			if(!BrandId.Value.Equals(cmd.BrandId.Value))
			{
				throw DomainException.CausedBy("Section does not have the same brandId as the brand which you are trying to add it to");
			}

			RaiseEvent(new SectionAdded(
				Id,
				cmd.BrandId,
				cmd.SectionId
			));

		}

		public void Execute(RemoveSection cmd)
		{
			if(Id == Guid.Empty)
			{
				throw DomainException.CausedBy("Can't remove section to a non-existing brandPage");
			}

			if(!BrandId.Value.Equals(cmd.BrandId.Value))
			{
				throw DomainException.CausedBy("Can't remove section which does not have the same brandId");
			}

			RaiseEvent(new SectionRemoved(
				Id,
				cmd.BrandId,
				cmd.SectionId
			));

		}

		protected override void RegisterHandlers()
		{
			Handle<BrandPageCreated>(evt =>
			{
				Id = evt.Id;
				BrandId = evt.BrandId;
				SectionIds = evt.SectionIds;
				Title = evt.Title;
				CardImgId = evt.CardImgId;
				HeaderImgId = evt.HeaderImgId;
				Published = evt.Publish;
				UITemplateIdentifier = evt.UiTemplateIdentifier;
			});

			Handle<BrandPagePublished>(evt =>
			{
				Published = evt.Publish;
			});

			Handle<BrandPageEdited>(evt =>
			{
				Title = evt.Title;
				CardImgId = evt.CardImgId;
				HeaderImgId = evt.HeaderImgId;
				UITemplateIdentifier = evt.UiTemplateIdentifier;
			});

			Handle<SectionAdded>(evt =>
			{
				if (SectionIds == null)
				{
					SectionIds = new List<AggregateId>();
				}
				
				SectionIds.Add(new AggregateId(evt.SectionId.Value));
			});

			Handle<SectionRemoved>(evt =>
			{
				if (SectionIds == null)
				{
					SectionIds = new List<AggregateId>();
				}

				SectionIds.RemoveAll(x => x.Value.Equals(evt.SectionId.Value));
			});
		}
	}
}
