using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Champagne;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.Aggregates.Brand;
using CM.Backend.Domain.Aggregates.Champagne.Events;
using SimpleSoft.Mediator;
using CM.Backend.Domain.Aggregates.ChampagneRoot;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Commands;
using CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Marten.Linq.SoftDeletes;
using Serilog;
using CreateChampagneFolder = CM.Backend.Domain.Aggregates.ChampagneRoot.Commands.CreateChampagneFolder;

namespace CM.Backend.Commands.Handlers
{
    public class ChampagneHandler : CommandHandlerBase, 
        ICommandHandler<CreateChampagne, IdResponse>,
	    ICommandHandler<AddChampagneProfile, Response>,
	    ICommandHandler<SetChampagnePublishingStatus, Response>,
	    ICommandHandler<EditChampagne, Response>,
	    ICommandHandler<EditChampagneProfile, Response>,
	    ICommandHandler<MigrateChampagne, IdResponse>
    {
        public ChampagneHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }
	    
	    private const string CreateNewEditionFolderAndAddAfterCreation = "CreateNewEditionFolderAndAddAfterCreation";
	    private const string AddToExistingEditionsFolder = "AddToExistingEditionsFolder";
	    private const string None = "None";
	    
        public async Task<IdResponse> HandleAsync(CreateChampagne cmd, CancellationToken ct)
        {
			//Validate that brandId is valid --> If this dont throw EventStoreException
	        var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);
	        
	        //Create champagne!
			var champagne = new Champagne();
	        
	        champagne.Execute(new Domain.Aggregates.Champagne.Commands.CreateChampagne(
		        new AggregateId(Guid.NewGuid()),
		        new BottleName(cmd.BottleName),
		        new AggregateId(cmd.BrandId),
		        new ImageId(cmd.BottleImgId),
		        new VintageInfo(cmd.IsVintage, cmd.VintageYear)));
	        
	        //Save new champagne to eventStore
			await AggregateRepo.StoreAsync(champagne);
	        
	        //Determine if further action should be taken.
	        if (cmd.CreateChampagneOption.Equals(CreateNewEditionFolderAndAddAfterCreation))
	        {
		        //Create a new champagneFolder and add the champagne directly
		        var champagneFolder = new ChampagneFolder();

		        FolderContentType contentType = null;
		        var champagnes = new HashSet<AggregateId>();
		        champagnes.Add(new AggregateId(champagne.Id));
		        if (cmd.IsVintage)
		        {
			        contentType = new FolderContentType("Vintage");
		        }
		        else
		        {
			        contentType = new FolderContentType("Non-Vintage");
		        }
		        
		        champagneFolder.Execute(new CreateChampagneFolder(
			        new AggregateId(Guid.NewGuid()),
			        new NotEmptyString(cmd.BottleName),
			        new AggregateId(cmd.BrandId),
			        new ImageId(cmd.BottleImgId),
			        contentType,
			        new FolderType("Editions"),
					champagnes,
			        cmd.IsOnDiscover));

		        await AggregateRepo.StoreAsync(champagneFolder);
	        }
	        else if (cmd.CreateChampagneOption.Equals(AddToExistingEditionsFolder))
	        {
		        //Add the new champagne diretly to a existing edition folder
		        var champagneFolder = await AggregateRepo.LoadAsync<ChampagneFolder>(cmd.ChampagneFolderId);

		        if (!champagneFolder.FolderType.Value.Equals("Editions"))
		        {
			        //We dont return error, since the purposes of the method is to create the champagne and if a simple error occurs adding to folder we will just have to try again for now
			        return new IdResponse(champagne.Id);
		        }
		        
		        champagneFolder.Execute(new AddChampagne(new AggregateId(champagne.Id)));

		        await AggregateRepo.StoreAsync(champagneFolder);
	        }
	        
            return new IdResponse(champagne.Id);
        }

		public async Task<Response> HandleAsync(SetChampagnePublishingStatus cmd, CancellationToken ct)
		{

			var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);

			if(!champagne.BrandId.Value.Equals(cmd.BrandId))
			{
				throw DomainException.CausedBy("BrandId mismatch exception, trying to publish bottle with different brandId");
			}

			champagne.Execute(new Domain.Aggregates.Champagne.Commands.SetChampagnePublishStatus(cmd.IsPublished));

			await AggregateRepo.StoreAsync(champagne);

			return Response.Success();
		}

		public async Task<Response> HandleAsync(EditChampagne cmd, CancellationToken ct)
		{
			var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);
			
			if(!champagne.BrandId.Value.Equals(cmd.BrandId))
			{
				throw DomainException.CausedBy("BrandId mismatch exception, trying to edit bottle with different brandId");
			}

			champagne.Execute(new Domain.Aggregates.Champagne.Commands.EditChampagne(
				new BottleName(cmd.BottleName),
				new ImageId(cmd.BottleImgId),
				new VintageInfo(cmd.IsVintage, cmd.VintageYear)));

			await AggregateRepo.StoreAsync(champagne);

			return Response.Success();
		}
	    
	    public async Task<Response> HandleAsync(AddChampagneProfile cmd, CancellationToken ct)
	    {
			var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);

		    if (!champagne.BrandId.Value.Equals(cmd.BrandId))
		    {
			    return new Response(false, null, "BrandId of champange of context is different from the one provided, editing may only be done by owning brand");
		    }

		    //Initializing all variables since they are required, but the value may be null, or more correct the bool IsUnknown = true.
		    GrapeAmount chardonnay = new GrapeAmount(cmd.Chardonnay);
		    GrapeAmount pinotNoir = new GrapeAmount(cmd.PinotNoir);
		    GrapeAmount pinotMeunier = new GrapeAmount(cmd.PinotMeunier);
		    AlchoholVol alchoholVol = new AlchoholVol(cmd.AlchoholVol);
		    DosageAmount dosageAmount = new DosageAmount(cmd.DosageAmount);
		    ReserveWineAmount reserveWineAmount = new ReserveWineAmount(cmd.ReserveWineAmount);
		    AgeingPotential ageingPotential = new AgeingPotential(cmd.AgeingPotential);
		    ServingTemp servingTemp = new ServingTemp(cmd.ServingTemp);
		    RedWineAmount redWineAmount = new RedWineAmount(cmd.RedWineAmount);

		    if (cmd.DisplayDosage == null || cmd.DisplayStyle == null || cmd.DisplayCharacter == null)
		    {
			    return new Response(false, null, "Display style, dosage and character may not be null");
		    }

		    var dosageCodes = cmd.DosageCodes;
		    dosageCodes.Add(cmd.DisplayDosage);
		    var dosage = new Dosage(cmd.DisplayDosage, dosageCodes);

		    var styleCodes = cmd.StyleCodes;
		    styleCodes.Add(cmd.DisplayStyle);
		    var style = new Style(cmd.DisplayStyle, styleCodes);

		    var characterCodes = cmd.CharacterCodes;
		    characterCodes.Add(cmd.DisplayCharacter);
		    var character = new Character(cmd.DisplayCharacter, characterCodes);
		    

		    champagne.Execute(new Domain.Aggregates.Champagne.Commands.AddChampagneProfile(
			    cmd.Appearance,
			    cmd.BlendInfo,
			    cmd.Taste,
			    cmd.FoodPairing,
			    cmd.Aroma,
			    cmd.BottleHistory,
			    dosage,
			    style,
			    character,
			    redWineAmount,
			    servingTemp,
			    ageingPotential,
			    reserveWineAmount,
			    dosageAmount,
			    alchoholVol,
			    pinotNoir,
			    pinotMeunier,
			    chardonnay));

		    await AggregateRepo.StoreAsync(champagne);
			
		    return new IdResponse(champagne.Id);

	    }

		public async Task<Response> HandleAsync(EditChampagneProfile cmd, CancellationToken ct)
		{
			var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);

			if (!champagne.BrandId.Value.Equals(cmd.BrandId))
		    {
			    return new Response(false, null, "BrandId of champange of context is different from the one provided, editing may only be done by owning brand");
		    }

		    //Initializing all variables since they are required, but the value may be null, or more correct the bool IsUnknown = true.
		    GrapeAmount chardonnay = new GrapeAmount(cmd.Chardonnay);
		    GrapeAmount pinotNoir = new GrapeAmount(cmd.PinotNoir);
		    GrapeAmount pinotMeunier = new GrapeAmount(cmd.PinotMeunier);
		    AlchoholVol alchoholVol = new AlchoholVol(cmd.AlchoholVol);
		    DosageAmount dosageAmount = new DosageAmount(cmd.DosageAmount);
		    ReserveWineAmount reserveWineAmount = new ReserveWineAmount(cmd.ReserveWineAmount);
		    AgeingPotential ageingPotential = new AgeingPotential(cmd.AgeingPotential);
		    ServingTemp servingTemp = new ServingTemp(cmd.ServingTemp);
		    RedWineAmount redWineAmount = new RedWineAmount(cmd.RedWineAmount);

		    if (cmd.DisplayDosage == null || cmd.DisplayStyle == null || cmd.DisplayCharacter == null)
		    {
			    return new Response(false, null, "Display style, dosage and character may not be null");
		    }

		    var dosageCodes = cmd.DosageCodes;
		    dosageCodes.Add(cmd.DisplayDosage);
		    var dosage = new Dosage(cmd.DisplayDosage, dosageCodes);

		    var styleCodes = cmd.StyleCodes;
		    styleCodes.Add(cmd.DisplayStyle);
		    var style = new Style(cmd.DisplayStyle, styleCodes);

		    var characterCodes = cmd.CharacterCodes;
		    characterCodes.Add(cmd.DisplayCharacter);
		    var character = new Character(cmd.DisplayCharacter, characterCodes);

		    champagne.Execute(new Domain.Aggregates.Champagne.Commands.EditChampagneProfile(
			    cmd.Appearance,
			    cmd.BlendInfo,
			    cmd.Taste,
			    cmd.FoodPairing,
			    cmd.Aroma,
			    cmd.BottleHistory,
			    dosage,
			    style,
			    character,
			    redWineAmount,
			    servingTemp,
			    ageingPotential,
			    reserveWineAmount,
			    dosageAmount,
			    alchoholVol,
			    pinotNoir,
			    pinotMeunier,
			    chardonnay));

		    await AggregateRepo.StoreAsync(champagne);
			
		    return new IdResponse(champagne.Id);
		}

	    public async Task<IdResponse> HandleAsync(MigrateChampagne cmd, CancellationToken ct)
	    {
		    
		    //Validate that brandId is valid --> If this dont throw EventStoreException
	        var brand = await AggregateRepo.LoadAsync<Brand>(cmd.CreateChampagne.BrandId);
	        
	        //Create champagne!
			var champagne = new Champagne();
	        
	        champagne.Execute(new Domain.Aggregates.Champagne.Commands.CreateChampagne(
		        new AggregateId(Guid.NewGuid()),
		        new BottleName(cmd.CreateChampagne.BottleName),
		        new AggregateId(cmd.CreateChampagne.BrandId),
		        new ImageId(cmd.CreateChampagne.BottleImgId),
		        new VintageInfo(cmd.CreateChampagne.IsVintage, cmd.CreateChampagne.VintageYear)));
	        
	        //Save new champagne to eventStore
			await AggregateRepo.StoreAsync(champagne);
	        
	        //Determine if further action should be taken.
	        if (cmd.CreateChampagne.CreateChampagneOption.Equals(CreateNewEditionFolderAndAddAfterCreation))
	        {
		        //Create a new champagneFolder and add the champagne directly
		        var champagneFolder = new ChampagneFolder();

		        FolderContentType contentType = null;
		        var champagnes = new HashSet<AggregateId>();
		        champagnes.Add(new AggregateId(champagne.Id));
		        if (cmd.CreateChampagne.IsVintage)
		        {
			        contentType = new FolderContentType("Vintage");
		        }
		        else
		        {
			        contentType = new FolderContentType("Non-Vintage");
		        }
		        
		        champagneFolder.Execute(new CreateChampagneFolder(
			        new AggregateId(Guid.NewGuid()),
			        new NotEmptyString(cmd.CreateChampagne.BottleName),
			        new AggregateId(cmd.CreateChampagne.BrandId),
			        new ImageId(cmd.CreateChampagne.BottleImgId),
			        contentType,
			        new FolderType("Editions"),
					champagnes,
			        false));

		        await AggregateRepo.StoreAsync(champagneFolder);
	        }
	        else if (cmd.CreateChampagne.CreateChampagneOption.Equals(AddToExistingEditionsFolder))
	        {
		        //Add the new champagne diretly to a existing edition folder
		        var champagneFolder = await AggregateRepo.LoadAsync<ChampagneFolder>(cmd.CreateChampagne.ChampagneFolderId);

		        if (!champagneFolder.FolderType.Value.Equals("Editions"))
		        {
			        //We dont return error, since the purposes of the method is to create the champagne and if a simple error occurs adding to folder we will just have to try again for now
			        return new IdResponse(champagne.Id);
		        }
		        
		        champagneFolder.Execute(new AddChampagne(new AggregateId(champagne.Id)));

		        await AggregateRepo.StoreAsync(champagneFolder);
	        }
	        
		    //***** ChampagneCreated *****
		    
		    //Initializing all variables since they are required, but the value may be null, or more correct the bool IsUnknown = true.
		    GrapeAmount chardonnay = new GrapeAmount(cmd.ChampagneProfile.Chardonnay);
		    GrapeAmount pinotNoir = new GrapeAmount(cmd.ChampagneProfile.PinotNoir);
		    GrapeAmount pinotMeunier = new GrapeAmount(cmd.ChampagneProfile.PinotMeunier);
		    AlchoholVol alchoholVol = new AlchoholVol(cmd.ChampagneProfile.AlchoholVol);
		    DosageAmount dosageAmount = new DosageAmount(cmd.ChampagneProfile.DosageAmount);
		    ReserveWineAmount reserveWineAmount = new ReserveWineAmount(cmd.ChampagneProfile.ReserveWineAmount);
		    AgeingPotential ageingPotential = new AgeingPotential(cmd.ChampagneProfile.AgeingPotential);
		    ServingTemp servingTemp = new ServingTemp(cmd.ChampagneProfile.ServingTemp);
		    RedWineAmount redWineAmount = new RedWineAmount(cmd.ChampagneProfile.RedWineAmount);

		    if (cmd.ChampagneProfile.DisplayDosage == null || cmd.ChampagneProfile.DisplayStyle == null || cmd.ChampagneProfile.DisplayCharacter == null)
		    {
			    return new IdResponse(Guid.Empty, false, null);
		    }

		    var dosageCodes = cmd.ChampagneProfile.DosageCodes;
		    dosageCodes.Add(cmd.ChampagneProfile.DisplayDosage);
		    var dosage = new Dosage(cmd.ChampagneProfile.DisplayDosage, dosageCodes);

		    var styleCodes = cmd.ChampagneProfile.StyleCodes;
		    styleCodes.Add(cmd.ChampagneProfile.DisplayStyle);
		    var style = new Style(cmd.ChampagneProfile.DisplayStyle, styleCodes);

		    var characterCodes = cmd.ChampagneProfile.CharacterCodes;
		    characterCodes.Add(cmd.ChampagneProfile.DisplayCharacter);
		    var character = new Character(cmd.ChampagneProfile.DisplayCharacter, characterCodes);
		    

		    champagne.Execute(new Domain.Aggregates.Champagne.Commands.AddChampagneProfile(
			    cmd.ChampagneProfile.Appearance,
			    cmd.ChampagneProfile.BlendInfo,
			    cmd.ChampagneProfile.Taste,
			    cmd.ChampagneProfile.FoodPairing,
			    cmd.ChampagneProfile.Aroma,
			    cmd.ChampagneProfile.BottleHistory,
			    dosage,
			    style,
			    character,
			    redWineAmount,
			    servingTemp,
			    ageingPotential,
			    reserveWineAmount,
			    dosageAmount,
			    alchoholVol,
			    pinotNoir,
			    pinotMeunier,
			    chardonnay));

		    await AggregateRepo.StoreAsync(champagne);
		    
		    champagne.Execute(new Domain.Aggregates.Champagne.Commands.SetChampagnePublishStatus(cmd.PublishChampagne.IsPublished));
			champagne.Execute(new SetMigrationSource(new MigrationSource(cmd.MigrationSource, cmd.SourceId)));
		    
		    await AggregateRepo.StoreAsync(champagne);
		    
		    return new IdResponse(champagne.Id);
		    
	    }
    }
}
