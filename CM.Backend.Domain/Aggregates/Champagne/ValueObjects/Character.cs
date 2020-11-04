using System;
using System.Collections.Generic;

namespace CM.Backend.Domain.Aggregates.Champagne.ValueObjects
{
    public class Character
    {
        private HashSet<string> _eligibleCharacterCodes = new HashSet<string>{GrandCru, PremierCru};
        
        private const string GrandCru = "GrandCru";
        private const string PremierCru = "PremierCru";
        
        public string DisplayCharacterCode { get; private set; }
        public HashSet<string> CharacterCodes { get; private set; }


        public Character(string displayCharacterCode, HashSet<string> characterCodes)
        {
            if (displayCharacterCode == null || characterCodes == null)
            {
                throw new ArgumentException(nameof(Character));
            }

            if (!_eligibleCharacterCodes.Contains(displayCharacterCode))
            {
                throw new ArgumentException(nameof(displayCharacterCode));
            }

            foreach (var characterCode in characterCodes)
            {
                if (!_eligibleCharacterCodes.Contains(displayCharacterCode))
                {
                    throw new ArgumentException(nameof(characterCodes));
                }
            }

            DisplayCharacterCode = displayCharacterCode;
            CharacterCodes = characterCodes;
        }
    }
}