using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface INotificationActivityDuplicateRepository : IMongoReadmodelRepository<NotificationActivityDuplicate>
    {
        Task<NotificationActivityDuplicate> GetByKey(NotificationActivityDuplicate.PrimaryKey key);
    }
    
    public class NotificationActivityDuplicateRepository : MongoReadmodelRepository<NotificationActivityDuplicate>, INotificationActivityDuplicateRepository
    {
        public NotificationActivityDuplicateRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task<NotificationActivityDuplicate> GetByKey(NotificationActivityDuplicate.PrimaryKey key)
        {
            return await ExecuteCmd(() =>DefaultCollection.Find(n =>
                n.Key.InvokedById == key.InvokedById && n.Key.InvokedOnId == key.InvokedOnId).SingleOrDefaultAsync(),
                $"{nameof(GetByKey)} - {nameof(NotificationActivityDuplicateRepository)}");
        }
    }
}