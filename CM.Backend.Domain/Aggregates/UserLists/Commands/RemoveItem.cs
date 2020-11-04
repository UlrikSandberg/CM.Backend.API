using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class RemoveItem
    {
        public AggregateId Item { get; }


        public RemoveItem(AggregateId itemToRemove)
        {
            if (itemToRemove == null)
            {
                throw new ArgumentException($"One or more values in {nameof(RemoveItem)} constructor is null");
            }

            Item = itemToRemove;
        }
    }
}