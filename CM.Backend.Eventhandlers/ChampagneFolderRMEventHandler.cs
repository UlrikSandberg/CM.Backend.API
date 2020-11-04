using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.Champagne.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderCreatedEvents;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderEditedEvents;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using Marten.Linq.SoftDeletes;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
	public class ChampagneFolderRMEventHandler : 
		IEventHandler<MessageEnvelope<ChampagneFolderCreated>>,
		IEventHandler<MessageEnvelope<ChampagneFolderEditted>>,
		IEventHandler<MessageEnvelope<ChampagneFolderDeleted>>,
		IEventHandler<MessageEnvelope<ChampagneAddedToFolder>>,
		IEventHandler<MessageEnvelope<ChampagneRemovedFromFolder>>,
		IEventHandler<MessageEnvelope<ChampagneFolderCreatedV2>>,
		IEventHandler<MessageEnvelope<ChampagneFolderEditedV2>>
	{
		private readonly IChampagneFolderRepository _champagneFolderRepository;

		public ChampagneFolderRMEventHandler(IChampagneFolderRepository champagneFolderRepository)
		{
			_champagneFolderRepository = champagneFolderRepository;
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneFolderCreated> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.Insert(new ChampagneFolder
			{
				Id = evt.Id,
				AuthorId = evt.Event.AuthorId.Value,
				ChampagneIds = evt.Event.ChampagneIds.ConverToGuidList().ToArray(),
				ContentType = evt.Event.ContentType.Value,
				DisplayImageId = evt.Event.DisplayImageId.Value,
				FolderName = evt.Event.FolderName.Value,
				FolderType = evt.Event.FolderType.Value
			});
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneFolderEditted> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.EditChampagneFolder(evt.Id, evt.Event.FolderName.Value,
				evt.Event.DisplayImageId.Value, evt.Event.ContentType.Value, true);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneFolderDeleted> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.Delete(evt.Id);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneAddedToFolder> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.AddChampagneToFolder(evt.Id, evt.Event.ChampagneId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneRemovedFromFolder> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.RemoveChampagneFromFolder(evt.Id, evt.Event.ChampagneId.Value);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneFolderCreatedV2> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.Insert(new ChampagneFolder
			{
				Id = evt.Id,
				AuthorId = evt.Event.AuthorId.Value,
				ChampagneIds = evt.Event.ChampagneIds.ConverToGuidList().ToArray(),
				ContentType = evt.Event.ContentType.Value,
				DisplayImageId = evt.Event.DisplayImageId.Value,
				FolderName = evt.Event.FolderName.Value,
				FolderType = evt.Event.FolderType.Value,
				IsOnDiscover = evt.Event.IsOnDiscover
			});
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneFolderEditedV2> evt, CancellationToken ct)
		{
			await _champagneFolderRepository.EditChampagneFolder(evt.Id, evt.Event.FolderName.Value,
				evt.Event.DisplayImageId.Value, evt.Event.ContentType.Value, evt.Event.IsOnDiscover);
		}
	}
}
