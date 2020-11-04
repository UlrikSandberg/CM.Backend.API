using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.BrandPageSections.Commands;
using CM.Backend.Domain.Aggregates.BrandPageSections.Events;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandPageSections
{
	public class BrandPageSection : Aggregate
    {
		public AggregateId BrandId { get; private set; }
		public string Title { get; private set; }
	    public string SubTitle { get; private set; }
		public string Body { get; private set; }
		public List<AggregateId> ChampagneIds { get; private set; } 
		public List<AggregateId> ImageIds { get; private set; } 
		public bool IsDeleted { get; private set; }
       
        public void Execute(CreateBrandPageSection cmd)
		{

			RaiseEvent(new BrandPageSectionCreated(
				cmd.Id.Value,
				cmd.BrandId,
				cmd.Title,
				cmd.SubTitle,
				cmd.Body,
				cmd.ChampagneIds,
				cmd.ImageIds,
				cmd.IsDeleted
			));         
		}

		public void Execute(DeleteBrandPageSection cmd)
		{
			RaiseEvent(new SectionDeleted(
				Id,
				cmd.BrandId,
				cmd.BrandPageId

			));
		}
		public void Execute(EditSection cmd)
		{
			if(Id == Guid.Empty)
			{
				throw DomainException.CausedBy("Can't edit a section which has not yet been saved");
			}
			if(IsDeleted)
			{
				throw DomainException.CausedBy("Can't edit a deleted section");
			}

			RaiseEvent(new SectionEdited(
				Id,
				cmd.Title,
				cmd.SubTitle,
				cmd.Body,
				cmd.ImageIds,
				cmd.ChampagneIds
			));

		}
	    
		protected override void RegisterHandlers()
		{
			Handle<BrandPageSectionCreated>(evt =>
			{
				Id = evt.Id;
				BrandId = evt.BrandId;
				Title = evt.Title;
				SubTitle = evt.SubTitle;
				Body = evt.Body;
				ChampagneIds = evt.ChampagneIds;
				ImageIds = evt.ImageIds;
				IsDeleted = evt.IsDeleted;
			});

			Handle<SectionDeleted>(evt =>
			{
				IsDeleted = true;
			});

			Handle<SectionEdited>(evt =>
			{
				Title = evt.Title;
				SubTitle = evt.SubTitle;
				Body = evt.Body;
				ImageIds = evt.ImageIds;
				ChampagneIds = evt.ChampagneIds;
			});                      
		}
	}
}
