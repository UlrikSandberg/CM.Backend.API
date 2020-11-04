using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Commands;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderEditedEvents;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderCreatedEvents;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;


namespace CM.Backend.Domain.Aggregates.ChampagneRoot
{
	public class ChampagneFolder : Aggregate
	{
		public NotEmptyString FolderName { get; private set; }
		public AggregateId AuthorId { get; private set; }
		public ImageId DisplayImageId { get; private set; }
		public bool IsDeleted { get; private set; }
		public FolderContentType ContentType { get; private set; }
		public FolderType FolderType { get; private set; }
		
		public bool IsOnDiscover { get; private set; }
		
		public HashSet<AggregateId> ChampagneIds { get; private set; }
		
        public ChampagneFolder()
        {
	        
		}

		public void Execute(CreateChampagneFolder cmd)
		{
			RaiseEvent(new ChampagneFolderCreatedV2(
				cmd.Id.Value,
				cmd.FolderName,
				cmd.AuthorId,
				cmd.DisplayImageId,
				cmd.IsDeleted,
				cmd.ContentType,
				cmd.FolderType,
				cmd.ChampagneIds,
				cmd.IsOnDiscover));
		}

        public void Execute(DeleteChampagneFolder cmd)
		{
			RaiseEvent(new ChampagneFolderDeleted(Id, cmd.IsDeleted, ChampagneIds));
		}

		public void Execute(EditChampagneFolder cmd)
		{
			if (IsDeleted)
			{
				throw new DomainException("Can't edit deleted folder");
			}
			
			RaiseEvent(new ChampagneFolderEditedV2(
				Id,
				cmd.FolderName,
				cmd.DisplayImageId,
				cmd.ContentType,
				cmd.IsOnDiscover));
		}

		public void Execute(AddChampagne cmd)
		{
			RaiseEvent(new ChampagneAddedToFolder(Id, cmd.ChampagneId, FolderType, ContentType));
		}

		public void Execute(RemoveChampagne cmd)
		{
			RaiseEvent(new ChampagneRemovedFromFolder(Id, cmd.ChampagneId));
		}

		protected override void RegisterHandlers()
		{
			Handle<ChampagneFolderCreated>(evt =>
			{
				Id = evt.Id;
				FolderName = evt.FolderName;
				AuthorId = evt.AuthorId;
				DisplayImageId = evt.DisplayImageId;
				ContentType = evt.ContentType;
				FolderType = evt.FolderType;
				ChampagneIds = evt.ChampagneIds;
				IsDeleted = evt.IsDeleted;
			});
			
			Handle<ChampagneFolderCreatedV2>(evt =>
			{
				Id = evt.Id;
				FolderName = evt.FolderName;
				AuthorId = evt.AuthorId;
				DisplayImageId = evt.DisplayImageId;
				ContentType = evt.ContentType;
				FolderType = evt.FolderType;
				ChampagneIds = evt.ChampagneIds;
				IsDeleted = evt.IsDeleted;
				IsOnDiscover = evt.IsOnDiscover;
			});

			Handle<ChampagneFolderDeleted>(evt =>
			{
				IsDeleted = evt.IsDeleted;
			});

			Handle<ChampagneFolderEditted>(evt =>
			{
				FolderName = evt.FolderName;
				DisplayImageId = evt.DisplayImageId;
				ContentType = evt.ContentType;
			});

			Handle<ChampagneFolderEditedV2>(evt =>
			{
				FolderName = evt.FolderName;
				DisplayImageId = evt.DisplayImageId;
				ContentType = evt.ContentType;
				IsOnDiscover = evt.IsOnDiscover;
			});
			

			Handle<ChampagneAddedToFolder>(evt => { ChampagneIds.Add(evt.ChampagneId); });
			Handle<ChampagneRemovedFromFolder>(evt => { ChampagneIds.Remove(evt.ChampagneId); });
		}
	}
}
