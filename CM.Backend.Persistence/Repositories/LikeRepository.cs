using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface ILikeRepository : IMongoReadmodelRepository<Like>
    {
        Task DeleteLike(Like.PrimaryKey key);
        Task<Like> GetLikeByKey(Like.PrimaryKey key);
        Task<int> CountLike(Guid likeToContextId);
        Task UpdateLikeByNameBatchAsync(Guid likeById, string likeByProfileName);
        Task UpdateLikeByProfileImageBatchAsync(Guid likeById, Guid likeByProfileImageId);
    }
    
    public class LikeRepository : MongoReadmodelRepository<Like>, ILikeRepository
    {
        public LikeRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
        {
        }

        public async Task DeleteLike(Like.PrimaryKey key)
        {
            var query = from e in DefaultCollection.AsQueryable<Like>()
                where e.Key.LikeById == key.LikeById
                where e.Key.LikeToContextId == key.LikeToContextId
                select e;

            foreach (var like in query)
            {
                await ExecuteCmd(() =>
                    DefaultCollection.FindOneAndDeleteAsync(k => k.Id == like.Id),
                    $"{nameof(DeleteLike)} - {nameof(LikeRepository)}");
            }
        }

        public async Task<Like> GetLikeByKey(Like.PrimaryKey key)
        {
            return await ExecuteCmd(() =>
                DefaultCollection.Find(l => l.Key == key).SingleOrDefaultAsync(),
                $"{nameof(GetLikeByKey)} - {nameof(LikeRepository)}");
        }

        public async Task<int> CountLike(Guid likeToContextId)
        {
            var count = await ExecuteCmd(() =>
                DefaultCollection.CountDocumentsAsync(l => l.LikeToContextId == likeToContextId),
                $"{nameof(CountLike)} - {nameof(LikeRepository)}");

            return (int) count;
        }

        public async Task UpdateLikeByNameBatchAsync(Guid likeById, string likeByProfileName)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(l => l.LikeById == likeById,
                    Update.Set(l => l.LikeByName, likeByProfileName)),
                $"{nameof(UpdateLikeByNameBatchAsync)} - {nameof(LikeRepository)}");
        }

        public async Task UpdateLikeByProfileImageBatchAsync(Guid likeById, Guid likeByProfileImageId)
        {
            await ExecuteCmd(() =>
                DefaultCollection.UpdateManyAsync(l => l.LikeById == likeById,
                    Update.Set(l => l.LikeByProfileImgId, likeByProfileImageId)),
                $"{nameof(UpdateLikeByProfileImageBatchAsync)} - {nameof(LikeRepository)}");
        }
    }
}