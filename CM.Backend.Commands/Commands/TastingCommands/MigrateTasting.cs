using CM.Backend.Documents.Responses;

namespace CM.Backend.Commands.Commands.TastingCommands
{
    public class MigrateTasting : CommandWithIdResponse
    {
        public CreateTasting CreateTastingCmd { get; }
        public string MigrationSource { get; }
        public string SourceId { get; }

        public MigrateTasting(CreateTasting createTastingCmd, string migrationSource, string sourceId)
        {
            CreateTastingCmd = createTastingCmd;
            MigrationSource = migrationSource;
            SourceId = sourceId;
        }
    }
}