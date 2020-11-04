using System;
using System.Collections.Generic;

namespace CM.Backend.Commands.Commands
{
	public class AddChampagneProfile : CommandWithIdResponse
    {
        
		public Guid BrandId { get; private set; }
		public Guid ChampagneId { get; set; }

		public string Appearance { get; private set; }
		public string BlendInfo { get; private set; }
		public string Taste { get; private set; }
		public string FoodPairing { get; private set; }
		public string Aroma { get; private set; }
		public string BottleHistory { get; private set; }
	    public string DisplayDosage { get; private set; }
	    public HashSet<string> DosageCodes { get; private set; }
	    public string DisplayStyle { get; private set; }
	    public HashSet<string> StyleCodes { get; private set; }
	    public string DisplayCharacter { get; private set; }
	    public HashSet<string> CharacterCodes { get; private set; }
	    public double RedWineAmount { get; private set; }
		public double ServingTemp { get; private set; }
		public double AgeingPotential { get; private set; }
		public double ReserveWineAmount { get; private set; }
		public double DosageAmount { get; private set; }
		public double AlchoholVol { get; private set; }
		public double PinotNoir { get; private set; }
		public double PinotMeunier { get; private set; }
		public double Chardonnay { get; private set; }

	    
		public AddChampagneProfile(Guid brandId, Guid champagneId, string appearance, string blendInfo, string taste, string foodPairing, string aroma, string bottleHistory, string displayDosage, HashSet<string> dosageCodes, string displayStyle, HashSet<string> styleCodes, string displayCharacter, HashSet<string> characterCodes, double redWineAmount, double servingTemp, double ageingPotential, double reserveWineAmount, double dosageAmount, double alchoholVol, double pinotNoir, double pinotMeunier, double chardonnay)
        {
            Chardonnay = chardonnay;
			PinotMeunier = pinotMeunier;
			PinotNoir = pinotNoir;
			AlchoholVol = alchoholVol;
			DosageAmount = dosageAmount;
			ReserveWineAmount = reserveWineAmount;
			AgeingPotential = ageingPotential;
			ServingTemp = servingTemp;
			RedWineAmount = redWineAmount;
			BottleHistory = bottleHistory;
	        DisplayDosage = displayDosage;
	        DosageCodes = dosageCodes;
	        DisplayStyle = displayStyle;
	        StyleCodes = styleCodes;
	        DisplayCharacter = displayCharacter;
	        CharacterCodes = characterCodes;
	        Aroma = aroma;
			FoodPairing = foodPairing;
			Taste = taste;
			BlendInfo = blendInfo;
			Appearance = appearance;
			BrandId = brandId;
			ChampagneId = champagneId;
		}
    }
}
