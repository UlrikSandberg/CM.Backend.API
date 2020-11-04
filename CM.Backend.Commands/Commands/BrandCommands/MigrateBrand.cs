using CM.Backend.Documents.Responses;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Commands.BrandCommands
{
    public class MigrateBrand : CommandWithIdResponse
    {
        public MigrateBrand(string brandName, string migrationSource, string sourceId)
        {
            BrandName = brandName;
            MigrationSource = migrationSource;
            SourceId = sourceId;
        }

        public string BrandName { get; set; }
        public string MigrationSource { get; set; }
        public string SourceId { get; set; }    }
}