using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class RemoveTastedChampagne
    {
        public AggregateId ChampagneId { get; private set; }

        public RemoveTastedChampagne(AggregateId champagneId)
        {
            if (champagneId == null)
            {
                throw new ArgumentException(nameof(champagneId));    
            }
            
            ChampagneId = champagneId;
        }
    }
}