using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Commands
{
    public class AddChampagne
    {
        public AggregateId ChampagneId { get; private set; }

        public AddChampagne(AggregateId champagneId)
        {
            ChampagneId = champagneId;
        }
    }
}