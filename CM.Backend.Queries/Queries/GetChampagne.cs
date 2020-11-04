using System;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries
{
    public class GetChampagne : Query<ChampagneQueryModel>
    {
        public Guid ChampagneId { get; private set; }
	    public Guid ReqUserId { get; private set; }

	    public GetChampagne(Guid champagneId, Guid reqUserId)
	    {
		    ChampagneId = champagneId;
		    ReqUserId = reqUserId;
	    }
    }
}