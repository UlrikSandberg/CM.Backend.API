using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.ValueObjects
{
    public class FolderType : SingleValueObject<string>
    {
        private const string Editions = "Editions";
        
        public FolderType(string value) : base(value)
        {
            if (!string.Equals(value, Editions))
            {
                throw new ArgumentException(nameof(value) + ": Incompatible folderType, accepted arguments are --> " + Editions);
            }
        }
    }
}