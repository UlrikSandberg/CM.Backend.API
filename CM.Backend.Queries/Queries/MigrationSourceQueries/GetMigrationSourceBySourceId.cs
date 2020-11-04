using CM.Backend.Persistence.Model.MigrationSource;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Queries.MigrationSourceQueries
{
    public class GetMigrationSourceBySourceId : Query<MigrationSource>
    {
        public string SourceId { get; }

        public GetMigrationSourceBySourceId(string sourceId)
        {
            SourceId = sourceId;
        }
    }
}