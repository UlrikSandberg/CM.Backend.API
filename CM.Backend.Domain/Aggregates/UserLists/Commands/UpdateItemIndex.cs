using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class UpdateItemIndex
    {
        public AggregateId ItemId { get; }
        public int IndexPosition { get; }

        public UpdateItemIndex(AggregateId itemId, int indexPosition)
        {
            if (itemId == null)
            {
                throw new ArgumentException($"One or more values in {nameof(UpdateItemIndex)} constructor is null");
            }

            ItemId = itemId;
            IndexPosition = indexPosition;
        }
    }
}