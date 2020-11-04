using System;
using CM.Backend.Queries.Model.TastingModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.TastingQueries
{
    public class GetChampagneTasting : Query<TastingModel>
    {
        public Guid TastingId { get; private set; }
        public Guid RequesterId { get; private set; }

        public GetChampagneTasting(Guid tastingId, Guid requesterId)
        {
            TastingId = tastingId;
            RequesterId = requesterId;
        }
    }
}