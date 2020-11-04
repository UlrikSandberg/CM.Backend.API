using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class BookmarkChampagne
    {
		public AggregateId ChampagneId { get; private set; }

		public BookmarkChampagne(AggregateId champagneId)
        {
	        if (champagneId == null)
	        {
		        throw new ArgumentException(nameof(champagneId));
	        }
            ChampagneId = champagneId;
		}
    }
}
