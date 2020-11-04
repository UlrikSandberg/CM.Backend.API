using System;
using CM.Backend.Persistence.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Model.UserModels
{
    public class GetPersistenceUserModel : Query<User>
    {
        public Guid UserId { get; private set; }

        public GetPersistenceUserModel(Guid userId)
        {
            UserId = userId;
        }
    }
}