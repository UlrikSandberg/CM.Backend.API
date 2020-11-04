using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.Events
{
	public class ChampagneProfileEdited : DomainEvent
    {
		public string Appearance { get; private set; }
	    public string BlendInfo { get; private set; }
	    public string Taste { get; private set; }
	    public string FoodPairing { get; private set; }
	    public string Aroma { get; private set; }
	    public string BottleHistory { get; private set; }
	    public Dosage Dosage { get; private set; }
	    public Style Style { get; private set; }
	    public Character Character { get; private set; }
	    public RedWineAmount RedWineAmount { get; private set; }
	    public ServingTemp ServingTemp { get; private set; }
	    public AgeingPotential AgeingPotential { get; private set; }
	    public ReserveWineAmount ReserveWineAmount { get; private set; }
	    public DosageAmount DosageAmount { get; private set; }
	    public AlchoholVol AlchoholVol { get; private set; }
	    public GrapeAmount PinotNoir { get; private set; }
	    public GrapeAmount PinotMeunier { get; private set; }
	    public GrapeAmount Chardonnay { get; private set; }


	    public ChampagneProfileEdited(Guid id, string appearance, string blendInfo, string taste, string foodPairing, string aroma, string bottleHistory, Dosage dosage, Style style, Character character, RedWineAmount redWineAmount, ServingTemp servingTemp, AgeingPotential ageingPotential, ReserveWineAmount reserveWineAmount, DosageAmount dosageAmount, AlchoholVol alchoholVol, GrapeAmount pinotNoir, GrapeAmount pinotMeunier, GrapeAmount chardonnay) : base(id)
	    {
		    Appearance = appearance;
		    BlendInfo = blendInfo;
		    Taste = taste;
		    FoodPairing = foodPairing;
		    Aroma = aroma;
		    BottleHistory = bottleHistory;
		    Dosage = dosage;
		    Style = style;
		    Character = character;
		    RedWineAmount = redWineAmount;
		    ServingTemp = servingTemp;
		    AgeingPotential = ageingPotential;
		    ReserveWineAmount = reserveWineAmount;
		    DosageAmount = dosageAmount;
		    AlchoholVol = alchoholVol;
		    PinotNoir = pinotNoir;
		    PinotMeunier = pinotMeunier;
		    Chardonnay = chardonnay;
	    }
    }
}
