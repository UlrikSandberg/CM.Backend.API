using System;

using CM.Backend.Domain.Aggregates.Champagne.Commands;
using CM.Backend.Domain.Aggregates.Champagne.Events;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Domain.Aggregates.Champagne.Entities;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne
{
    public class Champagne : Aggregate
    {
		public BottleName BottleName { get; private set; }
		public AggregateId BrandId { get; private set; }
		public ImageId BottleImageId { get; private set; }
		public VintageInfo VintageInfo { get; private set; }
		public bool IsPublished { get; private set; }
		  
	    public ChampagneProfile ChampagneProfile { get; private set; }
	   
	    public MigrationSource MigrationSource { get; private set; }
	    
	    public void Execute(CreateChampagne cmd)
		{
			RaiseEvent(new ChampagneCreated(
				cmd.Id.Value,
				cmd.BottleName,
				cmd.BrandId,
				cmd.BottleImgId,
				cmd.VintageInfo,
				cmd.IsPublished
			));
		}

		public void Execute(EditChampagne cmd)
		{
			RaiseEvent(new ChampagneEdited(Id, cmd.BottleName, cmd.BottleImgId, cmd.VintageInfo));
		}
        
		public void Execute(AddChampagneProfile cmd)
		{

			if(Id == null)
			{
				throw DomainException.CausedBy("Can't add champagne profile to non-existing brand");
			}

			if(ChampagneProfile != null)
			{
				throw DomainException.CausedBy("There is allready a champagne profile edit it instead");
			}

			RaiseEvent(new ChampagneProfileAdded(
				Id,
				cmd.Appearance,
				cmd.BlendInfo,
				cmd.Taste,
				cmd.FoodPairing,
				cmd.Aroma,
				cmd.BottleHistory,
				cmd.Dosage,
				cmd.Style,
				cmd.Character,
				cmd.RedWineAmount,
				cmd.ServingTemp,
				cmd.AgeingPotential,
				cmd.ReserveWineAmount,
				cmd.DosageAmount,
				cmd.AlchoholVol,
				cmd.PinotNoir,
				cmd.PinotMeunier,
				cmd.Chardonnay
			));
		}

		public void Execute(EditChampagneProfile cmd)
		{
			if(Id == null)
			{
				throw DomainException.CausedBy("Bottle has not yet been saved");
			}

			if(ChampagneProfile == null)
			{
				throw DomainException.CausedBy("You should first add the champagneProfile before trying to edit");
			}

			RaiseEvent(new ChampagneProfileEdited(
				Id,
				cmd.Appearance,
				cmd.BlendInfo,
				cmd.Taste,
				cmd.FoodPairing,
				cmd.Aroma,
				cmd.BottleHistory,
				cmd.Dosage,
				cmd.Style,
				cmd.Character,
				cmd.RedWineAmount,
				cmd.ServingTemp,
				cmd.AgeingPotential,
				cmd.ReserveWineAmount,
				cmd.DosageAmount,
				cmd.AlchoholVol,
				cmd.PinotNoir,
				cmd.PinotMeunier,
				cmd.Chardonnay
			));
		}

		public void Execute(SetChampagnePublishStatus cmd)
		{
			if (ChampagneProfile == null)
			{
				throw DomainException.CausedBy("Can't publish champagne without ChampagneProfile or filtersearch parameters");
			}
			
			RaiseEvent(new ChampagnePublishStatusChanged(Id, cmd.IsPublished, BrandId));
		}

	    public void Execute(SetMigrationSource cmd)
	    {
		    RaiseEvent(new MigrationSourceSet(Id, cmd.MigrationSource));
	    }

        protected override void RegisterHandlers()
        {
			Handle<ChampagneCreated>(evt =>
			{
				Id = evt.Id;
				BottleName = evt.BottleName;
				BrandId = evt.BrandId;
				BottleImageId = evt.BottleImgId;
				VintageInfo = evt.VintageInfo;
				IsPublished = evt.IsPublished;
			});

			Handle<ChampagneEdited>(evt =>
			{
				BottleName = evt.BottleName;
				BottleImageId = evt.BottleImgId;
				VintageInfo = evt.VintageInfo;
			});

			Handle<ChampagneProfileAdded>(evt =>
			{
				ChampagneProfile = new ChampagneProfile(
					evt.Appearance,
					evt.BlendInfo,
					evt.Taste,
					evt.FoodPairing,
					evt.Aroma,
					evt.BottleHistory,
					evt.Dosage,
					evt.Style,
					evt.Character,
					evt.RedWineAmount,
					evt.ServingTemp,
					evt.AgeingPotential,
					evt.ReserveWineAmount,
					evt.DosageAmount,
					evt.AlchoholVol,
					evt.PinotNoir,
					evt.PinotMeunier,
					evt.Chardonnay);	
			});

			Handle<ChampagneProfileEdited>(evt =>
			{
				ChampagneProfile = new ChampagneProfile(
					evt.Appearance,
					evt.BlendInfo,
					evt.Taste,
					evt.FoodPairing,
					evt.Aroma,
					evt.BottleHistory,
					evt.Dosage,
					evt.Style,
					evt.Character,
					evt.RedWineAmount,
					evt.ServingTemp,
					evt.AgeingPotential,
					evt.ReserveWineAmount,
					evt.DosageAmount,
					evt.AlchoholVol,
					evt.PinotNoir,
					evt.PinotMeunier,
					evt.Chardonnay
				);
			});


			Handle<ChampagnePublishStatusChanged>(evt =>
			{
				IsPublished = evt.IsPublished;
			});

	        Handle<MigrationSourceSet>(evt => { MigrationSource = evt.MigrationSource; });

        }
    }
}
