using System;
using System.Collections.Generic;
using System.Linq;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class Style
    {
        private HashSet<string> _eligibleStyleCodes = new HashSet<string> {Rose, BlancDeBlanc, BlancDeNoir, OnIce, TradBrut, TradSweet};
        
        private const string Rose = "Rose";
        private const string BlancDeBlanc = "BlancDeBlanc";
        private const string BlancDeNoir = "BlancDeNoir";
        private const string OnIce = "OnIce";
        private const string TradBrut = "TradBrut";
        private const string TradSweet = "TradSweet";
        
        public string DisplayStyleCode { get; private set; }
        public HashSet<string> StyleCodes { get; private set; }
        
        public Style(string displayStyleCode, HashSet<string> styleCodes)
        {
            if (displayStyleCode == null || styleCodes == null)
            {
                throw new ArgumentException(nameof(Style));
            }
            
            if (!_eligibleStyleCodes.Contains(displayStyleCode) )
            {
               throw new ArgumentException(nameof(displayStyleCode) + ": Incompatible styleCode");
            }

            if (styleCodes.Count < 1)
            {
                throw new ArgumentException(nameof(styleCodes));
            }

            foreach (var styleCode in styleCodes)
            {
                if (!_eligibleStyleCodes.Contains(displayStyleCode))
                {
                    throw new ArgumentException(nameof(styleCodes) + ": Incompatible styleCode");
                }
            }

            DisplayStyleCode = displayStyleCode;
            StyleCodes = styleCodes;
        }
    }
}