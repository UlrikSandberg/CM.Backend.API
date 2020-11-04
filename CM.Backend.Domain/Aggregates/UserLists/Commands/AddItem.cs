using System;
using System.Collections.Generic;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class AddItem
    {
        public AggregateId Item { get; }

        public AddItem(AggregateId item)
        {
            if (item == null)
            {
                throw new ArgumentException($"One or more values in {nameof(AddItem)} constructor is null");
            }

            Item = item;
        }
    }
}