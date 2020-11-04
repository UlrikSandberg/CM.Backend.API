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
	public interface IFollowRepository : IMongoReadmodelRepository<Follow>
	{
		Task DeleteFollow(Follow.PrimaryKey key);
		Task<int> CountFollowers(Guid userId);
		Task<Follow> GetFollowByKey(Follow.PrimaryKey key);
		Task<IEnumerable<Follow>> GetFollowersOfUserId(Guid userId);
		Task<IEnumerable<Follow>> GetFollowingOfUserId(Guid userId);
		Task UpdateNameBatchAsync(Guid userId, string username);
		Task UpdateProfileImageBatchAsync(Guid userId, Guid profileImageId);

	}
    
	public class FollowRepository : MongoReadmodelRepository<Follow>, IFollowRepository
	{
		public FollowRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
		{
		}

		public async Task DeleteFollow(Follow.PrimaryKey key)
		{

			var query = from e in DefaultCollection.AsQueryable<Follow>()
						where e.Key.FollowById == key.FollowById
						where e.Key.FollowToId == key.FollowToId
						select e;

            foreach(var follow in query)
			{
				await ExecuteCmd(() =>
					DefaultCollection.FindOneAndDeleteAsync(k => k.Id == follow.Id),
					$"{nameof(DeleteFollow)} - {nameof(FollowRepository)}");
			}
		}

		public async Task<int> CountFollowers(Guid userId)
		{
			var count = await ExecuteCmd(() =>
				DefaultCollection.CountDocumentsAsync(u => u.FollowToId == userId),
				$"{nameof(CountFollowers)} - {nameof(FollowRepository)}");

			return (int) count;
		}

		public async Task<Follow> GetFollowByKey(Follow.PrimaryKey key)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(f => f.Key == key).SingleOrDefaultAsync(),
				$"{nameof(GetFollowByKey)} - {nameof(FollowRepository)}");
		}

		public async Task<IEnumerable<Follow>> GetFollowersOfUserId(Guid userId)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(f => f.FollowToId == userId).ToListAsync(),
				$"{nameof(GetFollowersOfUserId)} - {nameof(FollowRepository)}");
		}

		public async Task<IEnumerable<Follow>> GetFollowingOfUserId(Guid userId)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(f => f.FollowById == userId).ToListAsync(),
				$"{nameof(GetFollowingOfUserId)} - {nameof(FollowRepository)}");
		}

		public async Task UpdateNameBatchAsync(Guid userId, string username)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(f => f.FollowById == userId, Update
					.Set(f => f.FollowByName, username)
					.Set(f => f.FollowToName, username)),
				$"{nameof(UpdateNameBatchAsync)} - {nameof(FollowRepository)}");
		}

		public async Task UpdateProfileImageBatchAsync(Guid userId, Guid profileImageId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(f => f.FollowById == userId,
					Update.Set(f => f.FollowByProfileImgId, profileImageId)),
				$"{nameof(UpdateProfileImageBatchAsync)} - {nameof(FollowRepository)}");
			await ExecuteCmd(() =>
				DefaultCollection.UpdateManyAsync(f => f.FollowToId == userId,
					Update.Set(f => f.FollowToProfileImgId, profileImageId)),
				$"{nameof(UpdateProfileImageBatchAsync)} - {nameof(FollowRepository)}");
		}
	}
}
