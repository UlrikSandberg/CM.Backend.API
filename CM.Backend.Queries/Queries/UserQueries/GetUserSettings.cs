using System;
using CM.Backend.Queries.Model.UserModels;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserQueries
{
    public class GetUserSettings : Query<UserSettingsModel>
    {
        public Guid UserId { get; private set; }

        public GetUserSettings(Guid userId)
        {
            UserId = userId;
        }
    }
}