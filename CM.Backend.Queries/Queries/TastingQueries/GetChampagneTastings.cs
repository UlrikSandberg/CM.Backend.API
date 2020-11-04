using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CM.Backend.Queries.Model.TastingModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.TastingQueries
{
    public class GetChampagneTastings : Query<IEnumerable<TastingModel>>
    {
        public Guid ChampagneId { get; private set; }
        public Guid RequesterId { get; private set; }
        public int Page { get; private set; }
        public int PageSize { get; private set; }
        public bool OrderByAscendingDate { get; private set; }

        public GetChampagneTastings(Guid champagneId, Guid requesterId, int page, int pageSize, bool orderByAscendingDate = false)
        {
            ChampagneId = champagneId;
            RequesterId = requesterId;
            Page = page;
            PageSize = pageSize;
            OrderByAscendingDate = orderByAscendingDate;
        }  
    }
}