using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Queries.MigrationSourceQueries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class MigrationSourceQueryHandler :
        IQueryHandler<GetMigrationSourceBySourceId, MigrationSource>
    {
        private readonly IMigrationSourceRepository _migrationSourceRepository;

        public MigrationSourceQueryHandler(IMigrationSourceRepository migrationSourceRepository)
        {
            _migrationSourceRepository = migrationSourceRepository;
        }

        public async Task<MigrationSource> HandleAsync(GetMigrationSourceBySourceId query, CancellationToken ct)
        {
            return await _migrationSourceRepository.GetMigrationSourceBySourceId(query.SourceId);
        }
    }
}