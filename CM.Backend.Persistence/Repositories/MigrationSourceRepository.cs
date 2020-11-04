using System;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model.MigrationSource;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface IMigrationSourceRepository : IMongoReadmodelRepository<MigrationSource>
    {
        Task<MigrationSource> GetMigrationSourceBySourceId(string sourceId);
    }
    
    public class MigrationSourceRepository : MongoReadmodelRepository<MigrationSource>, IMigrationSourceRepository
    {
        public MigrationSourceRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task<MigrationSource> GetMigrationSourceBySourceId(string sourceId)
        {
            return await ExecuteCmd(() =>
                DefaultCollection.Find(x => x.SourceId == sourceId).FirstOrDefaultAsync(),
                $"{nameof(GetMigrationSourceBySourceId)} - {nameof(MigrationSourceRepository)}");
        }
    }
}