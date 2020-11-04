namespace CM.Backend.Domain.Aggregates.Shared
{
    public class MigrationSource
    {
        public MigrationSource(string migrationSourceName, string sourceId)
        {
            MigrationSourceName = migrationSourceName;
            SourceId = sourceId;
        }

        public string MigrationSourceName { get; set; }
        public string SourceId { get; set; }
    }
}