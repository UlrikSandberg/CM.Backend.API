using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{

    public interface IUserCellarRepository : IMongoReadmodelRepository<UserCellar>
    {
        Task DeleteTastedChampagne(UserCellar.Key key);
        Task<IEnumerable<UserCellar>> GetUserCellarPaged(Guid userId, int page, int pageSize);
    }
    
    public class UserCellarRepository : MongoReadmodelRepository<UserCellar>, IUserCellarRepository
    {
        public UserCellarRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task DeleteTastedChampagne(UserCellar.Key key)
        {
            var query = from e in DefaultCollection.AsQueryable<UserCellar>()
                where e.PrimaryKey.UserId == key.UserId
                where e.PrimaryKey.ChampagneId == key.ChampagneId
                select e;

            foreach (var tastedChampagne in query)
            {
                await ExecuteCmd(() =>
                        DefaultCollection.FindOneAndDeleteAsync(k => k.Id == tastedChampagne.Id),
                    $"{nameof(DeleteTastedChampagne)} - {nameof(UserCellarRepository)}");
            }
        }

        public async Task<IEnumerable<UserCellar>> GetUserCellarPaged(Guid userId, int page, int pageSize)
        {
            return await ExecuteCmd(() =>
                DefaultCollection
                    .Find(c => c.UserId == userId)
                    .Skip(page * pageSize)
                    .Limit(pageSize)
                    .ToListAsync(),
                $"{nameof(GetUserCellarPaged)} - {nameof(UserCellarRepository)}");
        }
    }
}