namespace CM.Backend.Commands.Commands.UserCommands
{
    public class MigrateUser : CommandWithIdResponse
    {
        public CreateUser CreateUserCmd { get; }
        public string MigrationSource { get; }
        public string SourceId { get; }

        public MigrateUser(CreateUser createUserCmd, string migrationSource, string sourceId)
        {
            CreateUserCmd = createUserCmd;
            MigrationSource = migrationSource;
            SourceId = sourceId;
        }
    }
}