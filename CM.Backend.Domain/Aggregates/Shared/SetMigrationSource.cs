namespace CM.Backend.Domain.Aggregates.Shared
{
    public class SetMigrationSource
    {
        public SetMigrationSource(MigrationSource migrationSource)
        {
            MigrationSource = migrationSource;
        }

        public MigrationSource MigrationSource { get; set; }
        
        

    }
}