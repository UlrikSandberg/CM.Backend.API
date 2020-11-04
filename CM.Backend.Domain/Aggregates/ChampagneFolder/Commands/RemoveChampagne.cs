using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.ChampagneRoot.Commands
{
    public class RemoveChampagne
    {
        public AggregateId ChampagneId { get; private set; }

        public RemoveChampagne(AggregateId champagneId)
        {
            ChampagneId = champagneId;
        }
    }
}