using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Serializers;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.HelperQueries
{
    public class MigrateChampagneRatings : Query<IEnumerable<Guid>>
    {
    }
}