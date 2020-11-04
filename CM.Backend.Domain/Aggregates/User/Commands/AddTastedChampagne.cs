using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class AddTastedChampagne
    {
        public AggregateId ChampagneId { get; private set; }

        public AddTastedChampagne(AggregateId champagneId)
        {
            if (champagneId == null)
            {
                throw new ArgumentException(nameof(champagneId));
            }
            ChampagneId = champagneId;
        }
    }
}