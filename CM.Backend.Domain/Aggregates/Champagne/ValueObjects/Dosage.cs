using System;
using System.Collections.Generic;
using System.Linq;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class Dosage
    {
        private HashSet<string> _eligibleDosageCodes = new HashSet<string> {BrutNature, ExtraBrut, Brut, ExtraDry, Sec, DemiSec, Doux};
        
        private const string BrutNature = "BrutNature";
        private const string ExtraBrut = "ExtraBrut";
        private const string Brut = "Brut";
        private const string ExtraDry = "ExtraDry";
        private const string Sec = "Sec";
        private const string DemiSec = "DemiSec";
        private const string Doux = "Doux";
        
        public string DisplayDosage { get; private set; }
        public HashSet<string> DosageCodes { get; private set; }
        
        public Dosage(string displayDosage, HashSet<string> dosageCodes)
        {
            if (displayDosage == null || dosageCodes == null)
            {
                throw new ArgumentException(nameof(Dosage) + ": Parameter values may not be null");
            }
            
            if (!_eligibleDosageCodes.Contains(displayDosage))
            {
                throw new ArgumentException(nameof(displayDosage));
            }

            foreach (var dosageCode in dosageCodes)
            {
                if (!_eligibleDosageCodes.Contains(dosageCode))
                {
                    throw new ArgumentException(nameof(dosageCodes));
                }
            }

            DisplayDosage = displayDosage;
            DosageCodes = dosageCodes;
        }
    }
}