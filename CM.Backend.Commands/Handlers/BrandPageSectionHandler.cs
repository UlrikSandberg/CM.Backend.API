using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.BrandPage;
using SimpleSoft.Mediator;
using CM.Backend.Domain.Aggregates.BrandPageSections;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;

namespace CM.Backend.Commands.Handlers
{
    public class BrandPageSectionHandler : CommandHandlerBase,
        ICommandHandler<CreateAndAddSection, IdResponse>,
        ICommandHandler<DeleteSection, Response>,
        ICommandHandler<EditSection, IdResponse>,
        ICommandHandler<CreateSection, IdResponse>
    {
        public BrandPageSectionHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }

        public async Task<IdResponse> HandleAsync(CreateAndAddSection cmd, CancellationToken ct)
        {
            var brandPageSection = new BrandPageSection();

            brandPageSection.Execute(new Domain.Aggregates.BrandPageSections.Commands.CreateBrandPageSection(
                new AggregateId(Guid.NewGuid()),
                new AggregateId(cmd.BrandId),
                cmd.Title,
                cmd.SubTitle,
                cmd.Body,
                cmd.Champagnes.ToList(),
                cmd.Images.ToList()));

            await AggregateRepo.StoreAsync(brandPageSection);
            
            var brandPage = await AggregateRepo.LoadAsync<BrandPage>(cmd.BrandPageId);

            brandPage.Execute(
                new Domain.Aggregates.BrandPage.Commands.AddSection(
                    new AggregateId(cmd.BrandId),
                    new AggregateId(brandPageSection.Id)));

            await AggregateRepo.StoreAsync(brandPage);
            
            return new IdResponse(brandPageSection.Id);

        }

        public async Task<Response> HandleAsync(DeleteSection cmd, CancellationToken ct)
        {
            var brandPageSection = await AggregateRepo.LoadAsync<BrandPageSection>(cmd.SectionId);

            brandPageSection.Execute(new Domain.Aggregates.BrandPageSections.Commands.DeleteBrandPageSection(
                new AggregateId(cmd.BrandId),
                new AggregateId(cmd.BrandPageId),
                new AggregateId(cmd.SectionId)));

            await AggregateRepo.StoreAsync(brandPageSection);

            return Response.Success();
        }

        public async Task<IdResponse> HandleAsync(EditSection cmd, CancellationToken ct)
        {
            var section = await AggregateRepo.LoadAsync<BrandPageSection>(cmd.SectionId);

            var aggCmd =
                new Domain.Aggregates.BrandPageSections.Commands.EditSection(
                    cmd.Title, cmd.SubTitle, cmd.Body, cmd.ImageIds.ToList(),
                    cmd.ChampagneIds.ToList());

            section.Execute(aggCmd);

            await AggregateRepo.StoreAsync(section);

            return new IdResponse(cmd.Id);
        }

        public async Task<IdResponse> HandleAsync(CreateSection cmd, CancellationToken ct)
        {
            var section = new BrandPageSection();

            section.Execute(new Domain.Aggregates.BrandPageSections.Commands.CreateBrandPageSection(
                new AggregateId(Guid.NewGuid()),
                new AggregateId(cmd.BrandId),
                cmd.Title,
                cmd.SubTitle,
                cmd.Body,
                cmd.Champagnes.ToList(),
                cmd.Images.ToList()));
                    
            await AggregateRepo.StoreAsync(section);

            return new IdResponse(section.Id);
        }
    }
}