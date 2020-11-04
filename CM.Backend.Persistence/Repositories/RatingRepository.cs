using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model.RatingModel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface IRatingRepository : IMongoReadmodelRepository<RatingModel>
    {
        Task<EntityRatingInfo> GetEntityAverageRatingAndCount(Guid entityId);
        Task DeleteRatingModel(RatingModel.PrimaryKey key);
        Task UpdateRatingModelByContextId(Guid contextId, double rating);

        Task<RatingModel> GetByKey(RatingModel.PrimaryKey key);
    }
    
    public class RatingRepository : MongoReadmodelRepository<RatingModel>, IRatingRepository
    {
        public RatingRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task<EntityRatingInfo> GetEntityAverageRatingAndCount(Guid entityId)
        {
            var query = DefaultCollection.AsQueryable()
                .Where(x => x.Id.Equals(entityId))
                .GroupBy(x => x.Id)
                .Select(x => new EntityRatingInfo()
                {
                    RatingCount = x.Count(),
                    AverageRating = x.Average(v => v.Rating),
                    RatingValue = x.Sum(v => v.Rating)
                });

            var qResult = await query.Take(1).SingleOrDefaultAsync();

            if (qResult == null)
            {
                qResult = new EntityRatingInfo
                {
                    AverageRating = 0,
                    RatingCount = 0,
                    RatingValue = 0
                };
            }
            
            return qResult;
        }

        public async Task DeleteRatingModel(RatingModel.PrimaryKey key)
        {
            await DefaultCollection.DeleteOneAsync(x =>
                x.Key.UserId.Equals(key.UserId) && x.Key.EntityId.Equals(key.EntityId));
        }

        public async Task UpdateRatingModelByContextId(Guid contextId, double rating)
        {
            await DefaultCollection.UpdateOneAsync(x => x.ContextId.Equals(contextId),
                Update.Set(y => y.Rating, rating));
        }

        public async Task<RatingModel> GetByKey(RatingModel.PrimaryKey key)
        {
            return await DefaultCollection.Find(x =>
                x.Key.UserId.Equals(key.UserId) && x.Key.EntityId.Equals(key.EntityId)).SingleOrDefaultAsync();
        }
    }
}
