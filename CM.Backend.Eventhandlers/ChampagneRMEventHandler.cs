using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain;
using CM.Backend.Domain.Aggregates.Champagne.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events;
using CM.Backend.Domain.Aggregates.ChampagneRoot.Events.ChampagneFolderCreatedEvents;
using CM.Backend.Domain.Aggregates.Tasting.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class ChampagneRMEventHandler : 
	IEventHandler<MessageEnvelope<ChampagneCreated>>,
	IEventHandler<MessageEnvelope<ChampagneProfileAdded>>,
	IEventHandler<MessageEnvelope<ChampagnePublishStatusChanged>>,
	IEventHandler<MessageEnvelope<ChampagneEdited>>,
	IEventHandler<MessageEnvelope<ChampagneProfileEdited>>,
	IEventHandler<MessageEnvelope<TastingCreated>>,
	IEventHandler<MessageEnvelope<TastingEdited>>,
	IEventHandler<MessageEnvelope<TastingDeleted>>,
	IEventHandler<MessageEnvelope<ChampagneAddedToFolder>>,
	IEventHandler<MessageEnvelope<ChampagneRemovedFromFolder>>,
	IEventHandler<MessageEnvelope<ChampagneFolderDeleted>>,
	IEventHandler<MessageEnvelope<ChampagneFolderCreated>>
    {
        private readonly IChampagneRepository _repo;

        public ChampagneRMEventHandler(IChampagneRepository repo)
        {
            _repo = repo;
        }

        public async Task HandleAsync(MessageEnvelope<ChampagneCreated> evt, CancellationToken ct)
        {
			await _repo.Insert(new Champagne
			{
				Id = evt.Id,
				BottleName = evt.Event.BottleName.Value,
				BottleImgId = evt.Event.BottleImgId.Value,
				BrandId = evt.Event.BrandId.Value,
				AverageRating = 0.0,
				vintageInfo = new VintageInfo { IsVintage = evt.Event.VintageInfo.IsVintage, Year = evt.Event.VintageInfo.VintageYear },
				IsPublished = evt.Event.IsPublished,
				RatingDictionary = new Dictionary<string, double>(),
				RateCount = 0,
				RateValue = 0,
				IsUpdated = true
			});
        }

		public async Task HandleAsync(MessageEnvelope<ChampagneProfileAdded> evt, CancellationToken ct)
		{
			await _repo.AddChampagneProfile(evt.Id, new ChampagneProfile
			{
				Appearance = evt.Event.Appearance,
				BlendInfo = evt.Event.BlendInfo,
				Taste = evt.Event.Taste,
				FoodPairing = evt.Event.FoodPairing,
				Aroma = evt.Event.Aroma,
				BottleHistory = evt.Event.BottleHistory,
				DisplayDosage = evt.Event.Dosage.DisplayDosage,
				DosageCodes = evt.Event.Dosage.DosageCodes,
				DisplayStyle = evt.Event.Style.DisplayStyleCode,
				StyleCodes = evt.Event.Style.StyleCodes,
				DisplayCharacter = evt.Event.Character.DisplayCharacterCode,
				CharacterCodes = evt.Event.Character.CharacterCodes,
				RedWineAmount = evt.Event.RedWineAmount.Value,
				ServingTemp = evt.Event.ServingTemp.Value,
				AgeingPotential = evt.Event.AgeingPotential.Value,
				ReserveWineAmount = evt.Event.ReserveWineAmount.Value,
				DosageAmount = evt.Event.DosageAmount.Value,
				AlchoholVol = evt.Event.AlchoholVol.Value,
				PinotNoir = evt.Event.PinotNoir.Value,
				PinotMeunier = evt.Event.PinotMeunier.Value,
				Chardonnay = evt.Event.Chardonnay.Value
			});
		}
	    
	    public async Task HandleAsync(MessageEnvelope<ChampagneProfileEdited> evt, CancellationToken ct)
	    {
		    await _repo.EditChampagneProfile(evt.Id, new ChampagneProfile
		    {
			    Appearance = evt.Event.Appearance,
			    BlendInfo = evt.Event.BlendInfo,
			    Taste = evt.Event.Taste,
			    FoodPairing = evt.Event.FoodPairing,
			    Aroma = evt.Event.Aroma,
			    BottleHistory = evt.Event.BottleHistory,
			    DisplayDosage = evt.Event.Dosage.DisplayDosage,
			    DosageCodes = evt.Event.Dosage.DosageCodes,
			    DisplayStyle = evt.Event.Style.DisplayStyleCode,
			    StyleCodes = evt.Event.Style.StyleCodes,
			    DisplayCharacter = evt.Event.Character.DisplayCharacterCode,
			    CharacterCodes = evt.Event.Character.CharacterCodes,
			    RedWineAmount = evt.Event.RedWineAmount.Value,
			    ServingTemp = evt.Event.ServingTemp.Value,
			    AgeingPotential = evt.Event.AgeingPotential.Value,
			    ReserveWineAmount = evt.Event.ReserveWineAmount.Value,
			    DosageAmount = evt.Event.DosageAmount.Value,
			    AlchoholVol = evt.Event.AlchoholVol.Value,
			    PinotNoir = evt.Event.PinotNoir.Value,
			    PinotMeunier = evt.Event.PinotMeunier.Value,
			    Chardonnay = evt.Event.Chardonnay.Value
		    });
	    }
	    
		public async Task HandleAsync(MessageEnvelope<ChampagnePublishStatusChanged> evt, CancellationToken ct)
		{
			await _repo.SetChampagnePublishStatus(evt.Id, evt.Event.IsPublished);
		}

		public async Task HandleAsync(MessageEnvelope<ChampagneEdited> evt, CancellationToken ct)
		{
			await _repo.EditChampagne(
				evt.Id,
				evt.Event.BottleName.Value,
				evt.Event.BottleImgId.Value,
				new VintageInfo {IsVintage = evt.Event.VintageInfo.IsVintage, Year = evt.Event.VintageInfo.VintageYear
			}); 
		}

	    public async Task HandleAsync(MessageEnvelope<TastingCreated> evt, CancellationToken ct)
	    {
		    //Deprecated, instead the RatingRMEventHandler will update the required fields...
		    //await _repo.AddChampagneRating(evt.Event.ChampagneId.Value, evt.Event.Id, evt.Event.Rating.Value);
	    }

	    public async Task HandleAsync(MessageEnvelope<TastingEdited> evt, CancellationToken ct)
	    {
		    //Deprecated, instead the RatingRMEventHandler will update the required fields...
		    //await _repo.EditChampagneRating(evt.Event.ChampagneId.Value, evt.Event.Id, evt.Event.Rating.Value);
	    }

	    public async Task HandleAsync(MessageEnvelope<TastingDeleted> evt, CancellationToken ct)
	    {
		    //Deprecated, instead the RatingRMEventHandler will update the required fields...
		    //await _repo.DeleteChampagneRating(evt.Event.ChampagneId.Value, evt.Event.Id);
	    }

	    public async Task HandleAsync(MessageEnvelope<ChampagneAddedToFolder> evt, CancellationToken ct)
	    {
		    await _repo.AddFolderDependencies(evt.Event.ChampagneId.Value, evt.Id, evt.Event.FolderType.Value);
	    }

	    public async Task HandleAsync(MessageEnvelope<ChampagneRemovedFromFolder> evt, CancellationToken ct)
	    {
		    await _repo.RemoveFolderDependencies(evt.Event.ChampagneId.Value, evt.Id);
	    }

	    public async Task HandleAsync(MessageEnvelope<ChampagneFolderDeleted> evt, CancellationToken ct)
	    {
		    await _repo.RemoveFolderDependencies(new List<Guid>(evt.Event.FolderContent.ConverToGuidList()), evt.Id);
	    }

	    public async Task HandleAsync(MessageEnvelope<ChampagneFolderCreated> evt, CancellationToken ct)
	    {
		    if (evt.Event.ChampagneIds.ConverToGuidList().Any())
		    {
			    await _repo.AddFolderDependencies(new List<Guid>(evt.Event.ChampagneIds.ConverToGuidList()), evt.Id, evt.Event.FolderType.Value);
		    }
	    }
    }
}