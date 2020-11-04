using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Entities
{
    public class ChampagneProfile
    {
        public string Appearance { get; private set; }
		public string BlendInfo { get; private set; }
		public string Taste { get; private set; }
		public string FoodPairing { get; private set; }
		public string Aroma { get; private set; }
		public string BottleHistory { get; private set; }
	    public Dosage Dosage { get; private set; }
	    public Style Style { get; private set; }
	    public Character Character { get; private set; } //TODO : VO
	    public RedWineAmount RedWineAmount { get; private set; }
		public ServingTemp ServingTemp { get; private set; }
		public AgeingPotential AgeingPotential { get; private set; }
		public ReserveWineAmount ReserveWineAmount { get; private set; } //TODO : VO
		public DosageAmount DosageAmount { get; private set; }
		public AlchoholVol AlchoholVol { get; private set; }
		public GrapeAmount PinotNoir { get; private set; }
		public GrapeAmount PinotMeunier { get; private set; }
		public GrapeAmount Chardonnay { get; private set; }

		public ChampagneProfile(string appearance, string blendInfo, string taste, string foodPairing, string aroma, string bottleHistory, Dosage dosage, Style style, Character character, RedWineAmount redWineAmount, ServingTemp servingTemp, AgeingPotential ageingPotential, ReserveWineAmount reserveWineAmount, DosageAmount dosageAmount, AlchoholVol alchoholVol, GrapeAmount pinotNoir, GrapeAmount pinotMeunier, GrapeAmount chardonnay)
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
	        Dosage = dosage;
	        Style = style;
	        Character = character;
	        Aroma = aroma;
			FoodPairing = foodPairing;
			Taste = taste;
			BlendInfo = blendInfo;
			Appearance = appearance;
		}
    }
}
