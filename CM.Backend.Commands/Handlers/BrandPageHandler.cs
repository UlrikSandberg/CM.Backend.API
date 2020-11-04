using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.BrandPage;
using CM.Backend.Domain.Aggregates.BrandPage.ValueObject;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Handlers
{
	public class BrandPageHandler : CommandHandlerBase,
	ICommandHandler<CreateBrandPage, IdResponse>,
	ICommandHandler<SetPublishingStatusBrandPage, Response>,
	ICommandHandler<EditBrandPage, IdResponse>,
	ICommandHandler<AddSection, Response>,
	ICommandHandler<RemoveSection, Response>
    {
		
        public BrandPageHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
		{
		}

		public async Task<IdResponse> HandleAsync(CreateBrandPage cmd, CancellationToken ct)
		{
			var brandPage = new BrandPage();

			brandPage.Execute(new Domain.Aggregates.BrandPage.Commands.CreateBrandPage(
				new AggregateId(Guid.NewGuid()),
				new AggregateId(cmd.BrandId),
				new PageTitle(cmd.Title),
				new ImageId(cmd.CardImgId),
				new ImageId(cmd.HeaderImgId),
				new UITemplateIdentifier(cmd.UiTemplateIdentifier)
			));

			await AggregateRepo.StoreAsync(brandPage);
			Logger.Information("{@BrandPage} created", brandPage);

			return new IdResponse(brandPage.Id);

		}
        
		public async Task<Response> HandleAsync(SetPublishingStatusBrandPage cmd, CancellationToken ct)
		{
			var brandPage = await AggregateRepo.LoadAsync<BrandPage>(cmd.BrandPageId);

			brandPage.Execute(new Domain.Aggregates.BrandPage.Commands.PublishBrandPage(
				new AggregateId(cmd.BrandId),
				new AggregateId(cmd.BrandPageId),
				cmd.Publish
			));

			await AggregateRepo.StoreAsync(brandPage);
			Logger.Information("BrandPage publishing status for {BrandPageId} set to {@PublishingStatus}", brandPage.Id, cmd);

			return Response.Success();
		}

		public async Task<IdResponse> HandleAsync(EditBrandPage cmd, CancellationToken ct)
		{
			var brandPage = await AggregateRepo.LoadAsync<BrandPage>(cmd.PageId);

			brandPage.Execute(new Domain.Aggregates.BrandPage.Commands.EditBrandPage(
				new PageTitle(cmd.Title),
				new ImageId(cmd.CardImgId),
				new ImageId(cmd.HeaderImgId),
				new UITemplateIdentifier(cmd.UiTemplateIdentifier)));

			await AggregateRepo.StoreAsync(brandPage);

			return new IdResponse(cmd.PageId);
		}

        
		public async Task<Response> HandleAsync(AddSection cmd, CancellationToken ct)
		{

			var brandPage = await AggregateRepo.LoadAsync<BrandPage>(cmd.PageId);

			brandPage.Execute(new Domain.Aggregates.BrandPage.Commands.AddSection(
				new AggregateId(cmd.BrandId),
				new AggregateId(cmd.SectionId)));

			await AggregateRepo.StoreAsync(brandPage);

			return Response.Success();
		}

		public async Task<Response> HandleAsync(RemoveSection cmd, CancellationToken ct)
		{
			var brandPage = await AggregateRepo.LoadAsync<BrandPage>(cmd.PageId);

			brandPage.Execute(new Domain.Aggregates.BrandPage.Commands.RemoveSection(
				new AggregateId(cmd.BrandId),
				new AggregateId(cmd.SectionId)));

			await AggregateRepo.StoreAsync(brandPage);

			return Response.Success();
		}
	}
}
