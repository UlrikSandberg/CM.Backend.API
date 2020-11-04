namespace CM.Backend.Commands.Commands
{
    public class MigrateChampagne : CommandWithIdResponse
    {
        public MigrateChampagne(string migrationSource, string sourceId, CreateChampagne createChampagne, AddChampagneProfile champagneProfile, SetChampagnePublishingStatus publishChampagne)
        {
            MigrationSource = migrationSource;
            SourceId = sourceId;
            CreateChampagne = createChampagne;
            ChampagneProfile = champagneProfile;
            PublishChampagne = publishChampagne;
        }

        public string MigrationSource { get; set; }
        public string SourceId { get; set; }
        public CreateChampagne CreateChampagne { get; }
        public AddChampagneProfile ChampagneProfile { get; }
        public SetChampagnePublishingStatus PublishChampagne { get; }
    }
}