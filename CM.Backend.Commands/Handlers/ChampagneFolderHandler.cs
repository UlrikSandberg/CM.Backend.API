using System;
using System.Collections.Generic;
using System.Net.Mime;
using CM.Backend.Commands.Commands;
using CM.Backend.Documents.Responses;
using SimpleSoft.Mediator;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Domain.Aggregates.Champagne;
using CM.Backend.Domain.Aggregates.ChampagneRoot;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Commands;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using CreateChampagneFolder = CM.Backend.Commands.Commands.CreateChampagneFolder;
using DeleteChampagneFolder = CM.Backend.Commands.Commands.DeleteChampagneFolder;
using EditChampagneFolder = CM.Backend.Commands.Commands.EditChampagneFolder;

namespace CM.Backend.Commands.Handlers
{
	public class ChampagneFolderHandler : CommandHandlerBase,
	ICommandHandler<CreateChampagneFolder, IdResponse>,
	ICommandHandler<DeleteChampagneFolder, Response>,
	ICommandHandler<EditChampagneFolder, Response>,
	ICommandHandler<AddChampagneToFolder, Response>,
	ICommandHandler<RemoveChampagneFromFolder, Response>
	{
		public ChampagneFolderHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
		{
		}

		public async Task<IdResponse> HandleAsync(CreateChampagneFolder cmd, CancellationToken ct)
		{
			var champagneRoot = new ChampagneFolder();

			champagneRoot.Execute(new Domain.Aggregates.ChampagneRoot.Commands.CreateChampagneFolder(
				new AggregateId(Guid.NewGuid()),
				new NotEmptyString(cmd.FolderName),
				new AggregateId(cmd.AuthorId),
				new ImageId(cmd.DisplayImageId),
				new FolderContentType(cmd.FolderContentType),
				new FolderType(cmd.FolderType),
				new HashSet<AggregateId>(),
				cmd.IsOnDiscover));
            
			await AggregateRepo.StoreAsync(champagneRoot);

			return new IdResponse(champagneRoot.Id);

		}
        
		public async Task<Response> HandleAsync(DeleteChampagneFolder cmd, CancellationToken ct)
		{

			var champagneFolder = await AggregateRepo.LoadAsync<ChampagneFolder>(cmd.ChampagneFolderId);

			if(!champagneFolder.AuthorId.Value.Equals(cmd.AuthorId))
			{
				Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@ChampagneFolder} authorId did not match commands authorId", cmd, champagneFolder);
				return new Response(false, null, "Illegal operation, editing privilliges reserved for author only!");
			}

			champagneFolder.Execute(new Domain.Aggregates.ChampagneRoot.Commands.DeleteChampagneFolder());

			await AggregateRepo.StoreAsync(champagneFolder);

			return Response.Success();
            
		}

		public async Task<Response> HandleAsync(EditChampagneFolder cmd, CancellationToken ct)
		{
			var champagneFolder = await AggregateRepo.LoadAsync<ChampagneFolder>(cmd.ChampagneFolderId);

			if(!champagneFolder.AuthorId.Value.Equals(cmd.AuthorId))
			{
				Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@ChampagneFolder} authorId did not match commands authorId", cmd, champagneFolder);
				return new Response(false, null, "Illegal operation, editing privilliges reserved for author only!");
			}

			champagneFolder.Execute(new Domain.Aggregates.ChampagneRoot.Commands.EditChampagneFolder(
				new NotEmptyString(cmd.FolderName),
				new ImageId(cmd.DisplayImageId),
				new FolderContentType(cmd.FolderContentType),
				cmd.IsOnDiscover));

			await AggregateRepo.StoreAsync(champagneFolder);

			return Response.Success();
		}

		public async Task<Response> HandleAsync(AddChampagneToFolder cmd, CancellationToken ct)
		{
			//Check if champagne exists 
			var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);
			if (champagne == null)
			{
				return new Response(false, null, "Not a valid champagneId");
			}
			var champagneFolder = await AggregateRepo.LoadAsync<ChampagneFolder>(cmd.ChampagneFolderId);

			if (!champagneFolder.AuthorId.Value.Equals(cmd.AuthorId))
			{
				Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@ChampagneFolder} authorId did not match commands authorId", cmd, champagneFolder);
				return new Response(false, null, "Illegal operation, editing privilliges reserved for author only!");
			}
			
			champagneFolder.Execute(new AddChampagne(new AggregateId(cmd.ChampagneId)));

			await AggregateRepo.StoreAsync(champagneFolder);
			
			return Response.Success();
		}

		public async Task<Response> HandleAsync(RemoveChampagneFromFolder cmd, CancellationToken ct)
		{
			//Check if champagne exists 
			var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);
			if (champagne == null)
			{
				return new Response(false, null, "Not a valid champagneId");
			}
			var champagneFolder = await AggregateRepo.LoadAsync<ChampagneFolder>(cmd.ChampagneFolderId);
			
			
			if (!champagneFolder.AuthorId.Value.Equals(cmd.AuthorId))
			{
				Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@ChampagneFolder} authorId did not match commands authorId", cmd, champagneFolder);
				return new Response(false, null, "Illegal operation, editing privilliges reserved for author only!");
			}
			
			champagneFolder.Execute(new RemoveChampagne(new AggregateId(cmd.ChampagneId)));

			await AggregateRepo.StoreAsync(champagneFolder);
			
			return Response.Success();

		}
	}
}
