using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands;
using CM.Backend.Commands.EnumOptions;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Handlers
{
	public class BrandImageHandler : CommandHandlerBase,
	ICommandHandler<UploadBrandImage, IdResponse>
	{
		private readonly IBrandTypes brandTypes;

		public BrandImageHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
		{
			this.brandTypes = new BrandTypes(); 
		}

		public async Task<IdResponse> HandleAsync(UploadBrandImage cmd, CancellationToken ct)
		{

			var image = new BrandImage();

			image.Execute(new Domain.Aggregates.BrandFile.BrandImage.Commands.UploadImage(
				new AggregateId(cmd.ImageId),
				new AggregateId(cmd.BrandId),
				new ImageName(cmd.ImageName),
				new TypeOfBrandImage(cmd.TypeOfBrandImage),
				new FileType(cmd.FileExtension)));

			await AggregateRepo.StoreAsync(image);
			Logger.Information("{@BrandImage} stored", image);

			return new IdResponse(image.Id);
            

		}
	}
}
