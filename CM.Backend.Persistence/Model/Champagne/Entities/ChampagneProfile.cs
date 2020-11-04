using System.Collections.Generic;

namespace CM.Backend.Persistence.Model.Entities
{
    public class ChampagneProfile
    {
        public string Appearance { get; set; }
        public string BlendInfo { get; set; }
        public string Taste { get; set; }
        public string FoodPairing { get; set; }
        public string Aroma { get; set; }
        public string BottleHistory { get; set; }
		
        public string DisplayDosage { get; set; }
        public HashSet<string> DosageCodes { get; set; }
        public string DisplayStyle { get; set; }
        public HashSet<string> StyleCodes { get; set; }
        public string DisplayCharacter { get; set; }
        public HashSet<string> CharacterCodes { get; set; }
		
        public double RedWineAmount { get; set; }
        public double ServingTemp { get; set; }
        public double AgeingPotential { get; set; }
        public double ReserveWineAmount { get; set; }
        public double DosageAmount { get; set; }
        public double AlchoholVol { get; set; }
        public double PinotNoir { get; set; }
        public double PinotMeunier { get; set; }
        public double Chardonnay { get; set; }
    }
}