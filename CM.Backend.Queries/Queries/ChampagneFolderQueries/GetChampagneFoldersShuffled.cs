using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetChampagneFoldersShuffled : Query<IEnumerable<ChampagneFolderQueryModel>>
    {
        public bool IsShuffled { get; private set; }
        public int Amount { get; private set; }

        public GetChampagneFoldersShuffled(bool isShuffled, int amount)
        {
            IsShuffled = isShuffled;
            Amount = amount;
        }
    }
}