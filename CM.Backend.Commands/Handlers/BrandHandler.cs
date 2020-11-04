using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands;
using CM.Backend.Commands.Commands.BrandCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Brand;
using DomainCommands = CM.Backend.Domain.Aggregates.Brand.Commands;
using CM.Backend.Domain.Aggregates.Brand.Entities;
using CM.Backend.Domain.Aggregates.Brand.Events;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Serilog;
using SimpleSoft.Mediator;
using SetPublishingStatusBrand = CM.Backend.Commands.Commands.SetPublishingStatusBrand;

namespace CM.Backend.Commands.Handlers
{
    public class BrandHandler : CommandHandlerBase,
	    ICommandHandler<CreateBrand, IdResponse>,
	    ICommandHandler<UpdateBrandInfo, Response>,
	    ICommandHandler<MigrateBrand, IdResponse>,
	    ICommandHandler<UpdateBrandImages, Response>,
	    ICommandHandler<UpdateBrandSocial, Response>,
	    ICommandHandler<UpdateBrandCellar, Response>,
	    ICommandHandler<SetPublishingStatusBrand, Response>,
	    ICommandHandler<CreateAndAddBrandCellarSection, IdResponse>,
	    ICommandHandler<DeleteBrandCellarSection, Response>,
	    ICommandHandler<UpdateBrandCellarSection, Response>
    {
	    
        public BrandHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }

        public async Task<IdResponse> HandleAsync(CreateBrand cmd, CancellationToken ct)
        {
            var brand = new Brand();
	        
	        brand.Execute(new DomainCommands.CreateBrand(
		        new AggregateId(Guid.NewGuid()),
		        new BrandName(cmd.Name), 
		        cmd.ProfileText,
		        new ImageId(Guid.Empty),
		        new ImageId(Guid.Empty),
		        new ImageId(Guid.Empty),
		        new ImageId(Guid.Empty),
		        new InstagramUrl(cmd.InstagramUrl),
		        new FacebookUrl(cmd.FacebookUrl),
		        new PinterestUrl(cmd.PinterestUrl),
		        new TwitterUrl(cmd.TwitterUrl),
		        new UrlValueObject(cmd.WebsiteUrl),
		        new Cellar
			        ("Cellar",
			        new ImageId(Guid.Empty),
			        new ImageId(Guid.Empty),
			        new List<CellarSection>())
		        ));
			
            await AggregateRepo.StoreAsync(brand);
	        Logger.Information("{@Brand} stored", brand);

            return new IdResponse(brand.Id);
        }
	    
	    public async Task<Response> HandleAsync(UpdateBrandInfo cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);
		    
		    brand.Execute(new DomainCommands.UpdateBrandInfo(new BrandName(cmd.Name), cmd.ProfileText));

		    await AggregateRepo.StoreAsync(brand);
		    
		    Logger.Information("{@BrandInfo} updated", cmd);
		    
		    return Response.Success();
	    }
	    
	    public async Task<Response> HandleAsync(UpdateBrandImages cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);

		    brand.Execute(new DomainCommands.UpdateBrandImages(
			    new ImageId(cmd.BrandCoverImageId),
			    new ImageId(cmd.BrandListImageId),
			    new ImageId(cmd.BottleCoverImageId),
			    new ImageId(cmd.LogoImageId)));

		    await AggregateRepo.StoreAsync(brand);
		    
		    Logger.Information("{@BrandImages} updated", cmd);
		    
		    return Response.Success();
	    }
	    
	    public async Task<Response> HandleAsync(UpdateBrandSocial cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);
		    
		    brand.Execute(new DomainCommands.UpdateSocial(
			    new InstagramUrl(cmd.InstagramUrl),
			    new FacebookUrl(cmd.FacebookUrl),
			    new PinterestUrl(cmd.PinterestUrl),
			    new TwitterUrl(cmd.TwitterUrl),
			    new UrlValueObject(cmd.WebsiteUrl)));

		    await AggregateRepo.StoreAsync(brand);
		    
		    Logger.Information("{@BrandSocial} updated", cmd);
		    
		    return Response.Success();
	    }
	    
	    public async Task<Response> HandleAsync(UpdateBrandCellar cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);
		    
		    brand.Execute(new DomainCommands.UpdateCellar(
			    new ImageId(cmd.CardImageId),
			    new ImageId(cmd.CoverImageId)));

		    await AggregateRepo.StoreAsync(brand);
		    
		    Logger.Information("{@BrandCellar} update", cmd);
		    
		    return Response.Success();
	    }
	    
	    public async Task<IdResponse> HandleAsync(CreateAndAddBrandCellarSection cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);

		    var sectionId = Guid.NewGuid();
		    
			brand.Execute(new DomainCommands.CreateAndAddCellarSection(
				new AggregateId(sectionId),
				cmd.Title,
				cmd.Body,
				cmd.Champagnes));

		    await AggregateRepo.StoreAsync(brand);
		    
		    return new IdResponse(sectionId);
	    }
	    
	    public async Task<Response> HandleAsync(DeleteBrandCellarSection cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);
		    
		    //Check if brand contains cellarSection with given id;
		    foreach (var section in brand.Cellar.Sections)
		    {
			    if (section.Id.Value.Equals(cmd.SectionId))
			    {
				    break;
			    }
			    
			    return new Response(false, null, "No cellar section with id:" + cmd.SectionId + " exists for brandId: " + cmd.BrandId);
		    }
		    
		    brand.Execute(new DomainCommands.DeleteCellarSection(new AggregateId(cmd.SectionId)));

		    await AggregateRepo.StoreAsync(brand);
		    Logger.Information("CellarSection deleted from {BrandId}", brand.Id);
		    
		    return Response.Success();
	    }
	    
	    public async Task<Response> HandleAsync(SetPublishingStatusBrand cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);

		    brand.Execute(new DomainCommands.SetPublishingStatusBrand(
			    new AggregateId(cmd.BrandId), cmd.PublishStatus));

		    await AggregateRepo.StoreAsync(brand);
		    Logger.Information("Brand publishing status for {BrandId} set to {@PublishingStatus}", brand.Id, cmd);
			
		    return Response.Success();
	    }
	    
	    public async Task<Response> HandleAsync(UpdateBrandCellarSection cmd, CancellationToken ct)
	    {
		    var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);
		    
		    brand.Execute(new DomainCommands.UpdateCellarSection(
			    new AggregateId(cmd.SectionId),
			    cmd.Title,
			    cmd.Body,
			    cmd.ChampagneIds));

		    await AggregateRepo.StoreAsync(brand);
		    
		    Logger.Information("Brand cellar section {@SectionId} updated with {@Update}", cmd.SectionId, cmd);

		    return Response.Success();

	    }
	    
	    public async Task<IdResponse> HandleAsync(MigrateBrand cmd, CancellationToken ct)
	    {
		    var brandId = Guid.NewGuid();
		    var brand = new Brand();
		    brand.Execute(new DomainCommands.CreateBrand(
			    new AggregateId(brandId),
			    new BrandName(cmd.BrandName), 
			    "",
			    new ImageId(Guid.Empty),
			    new ImageId(Guid.Empty),
			    new ImageId(Guid.Empty),
			    new ImageId(Guid.Empty),
			    new InstagramUrl(null),
			    new FacebookUrl(null),
			    new PinterestUrl(null),
			    new TwitterUrl(null),
			    new UrlValueObject(null),
			    new Cellar
			    ("Cellar",
				    new ImageId(Guid.Empty),
				    new ImageId(Guid.Empty),
				    new List<CellarSection>())
		    ));
		    
		    brand.Execute(new SetMigrationSource(new MigrationSource(cmd.MigrationSource, cmd.SourceId)));

		    await AggregateRepo.StoreAsync(brand);
		    
		    return new IdResponse(brandId);
	    }
    }
}