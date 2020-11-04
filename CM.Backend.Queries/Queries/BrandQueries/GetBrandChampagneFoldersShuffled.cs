using System;
using System.Collections.Generic;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetBrandChampagneFoldersShuffled : Query<IEnumerable<ChampagneFolderQueryModel>>
    {
        public Guid BrandId { get; private set; }
        public bool IsShuffled { get; private set; }
        public int Amount { get; private set; }

        public GetBrandChampagneFoldersShuffled(Guid brandId, bool isShuffled, int amount)
        {
            BrandId = brandId;
            IsShuffled = isShuffled;
            Amount = amount;
        }
        
    }
}