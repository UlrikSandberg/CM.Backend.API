using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UnbookmarkChampagne
    {
		public AggregateId ChampagneId { get; private set; }

		public UnbookmarkChampagne(AggregateId champagneId)
        {
	        if (champagneId == null)
	        {
		        throw new ArgumentException(nameof(champagneId));
	        }
	        
            ChampagneId = champagneId;
		}
    }
}
