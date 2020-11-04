using System.Collections;
using System.Collections.Generic;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.UserListQueries
{
    public class GetStandardTop10List : Query<IEnumerable<ChampagneLight>>
    {
        public string ConfigurationKey { get; }
        public bool FilterByVintage { get; }
        public bool FilterByHighestRating { get; }

        public GetStandardTop10List(string configurationKey, bool filterByVintage, bool filterByHighestRating)
        {
            ConfigurationKey = configurationKey;
            FilterByVintage = filterByVintage;
            FilterByHighestRating = filterByHighestRating;
        }
    }
}