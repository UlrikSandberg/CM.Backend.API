using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.TastingCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Champagne;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.Aggregates.Tasting;
using CM.Backend.Domain.Aggregates.Tasting.ValueObjects;
using CM.Backend.Domain.Aggregates.User;
using CM.Backend.Domain.Aggregates.User.Commands;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Handlers
{
    public class TastingHandler : CommandHandlerBase,
        ICommandHandler<CreateTasting, IdResponse>,
        ICommandHandler<EditTasting, Response>,
        ICommandHandler<DeleteTasting, Response>,
        ICommandHandler<MigrateTasting, IdResponse>
    {
        public TastingHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }

        public async Task<IdResponse> HandleAsync(CreateTasting cmd, CancellationToken ct)
        {
            var tasting = new Tasting();

            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            if (user.TastedChampagnes.Contains(new AggregateId(cmd.ChampagneId)))
            {
                return new IdResponse(Guid.Empty, false, new DomainException("This champagne has already been tasted"));
            }
            
            var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);

            if (champagne == null)
            {
                throw new DomainException("ChampagneId invalid");
            }

            tasting.Execute(new Domain.Aggregates.Tasting.Commands.CreateTasting(
                new AggregateId(Guid.NewGuid()),
                cmd.Review,
                new Rating(cmd.Rating),
                new AggregateId(cmd.UserId),
                new AggregateId(cmd.ChampagneId),
                new AggregateId(champagne.BrandId.Value),
                DateTime.UtcNow,
                true));

            await AggregateRepo.StoreAsync(tasting);
            
            //The champagne has been tasted add to user aggregate
            
            user.Execute(new AddTastedChampagne(new AggregateId(cmd.ChampagneId)));
            await AggregateRepo.StoreAsync(user);
            
            return new IdResponse(tasting.Id);
        }

        public async Task<Response> HandleAsync(EditTasting cmd, CancellationToken ct)
        {
            var tasting = await AggregateRepo.LoadAsync<Tasting>(cmd.TastingId);

            if (!tasting.AuthorId.Value.Equals(cmd.UserId))
            {
                Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@Tasting} authorId did not match commands authorId", cmd, tasting);
                return new Response(false, null, "Editing privileges is only reserved for the author");
            }
            
            tasting.Execute(new Domain.Aggregates.Tasting.Commands.EditTasting(cmd.Review, new Rating(cmd.Rating)));

            await AggregateRepo.StoreAsync(tasting);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(DeleteTasting cmd, CancellationToken ct)
        {
            var tasting = await AggregateRepo.LoadAsync<Tasting>(cmd.TastingId);
            
            if (!tasting.AuthorId.Value.Equals(cmd.UserId))
            {
                Logger.Error("Illegal operation, editing priviliges reserved for author only {@Command} was rejected since {@Tasting} authorId did not match commands authorId", cmd, tasting);
                return new Response(false, null, "Editing privileges is only reserved for the author");
            }
            
            tasting.Execute(new Domain.Aggregates.Tasting.Commands.DeleteTasting(true));

            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            await AggregateRepo.StoreAsync(tasting);
            
            user.Execute(new RemoveTastedChampagne(tasting.ChampagneId));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<IdResponse> HandleAsync(MigrateTasting cmd, CancellationToken ct)
        {
            var tasting = new Tasting();

            var user = await AggregateRepo.LoadAsync<User>(cmd.CreateTastingCmd.UserId);

            if (user.TastedChampagnes.Contains(new AggregateId(cmd.CreateTastingCmd.ChampagneId)))
            {
                return new IdResponse(Guid.Empty, false, new DomainException("This champagne has already been tasted"));
            }
            
            var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.CreateTastingCmd.ChampagneId);

            if (champagne == null)
            {
                throw new DomainException("ChampagneId invalid");
            }

            tasting.Execute(new Domain.Aggregates.Tasting.Commands.CreateTasting(
                new AggregateId(Guid.NewGuid()),
                cmd.CreateTastingCmd.Review,
                new Rating(cmd.CreateTastingCmd.Rating),
                new AggregateId(cmd.CreateTastingCmd.UserId),
                new AggregateId(cmd.CreateTastingCmd.ChampagneId),
                new AggregateId(champagne.BrandId.Value),
                cmd.CreateTastingCmd.TimeStamp,
                true));
            
            tasting.Execute(new SetMigrationSource(new MigrationSource(cmd.MigrationSource, cmd.SourceId)));

            await AggregateRepo.StoreAsync(tasting);
            
            //The champagne has been tasted add to user aggregate
            user.Execute(new AddTastedChampagne(new AggregateId(cmd.CreateTastingCmd.ChampagneId)));
            
            await AggregateRepo.StoreAsync(user);
            
            return new IdResponse(tasting.Id);
        }
    }
}