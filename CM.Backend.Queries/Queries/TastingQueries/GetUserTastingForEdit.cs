using System;
using CM.Backend.Queries.Model.TastingModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.TastingQueries
{
    public class GetUserTastingForEdit : Query<EditTastingModel>
    {
        public Guid UserId { get; private set; }
        public Guid ChampagneId { get; private set; }

        public GetUserTastingForEdit(Guid userId, Guid champagneId)
        {
            UserId = userId;
            ChampagneId = champagneId;
        }
        
    }
}