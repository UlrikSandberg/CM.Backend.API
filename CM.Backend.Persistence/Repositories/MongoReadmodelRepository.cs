using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CM.Backend.Documents.GlobalLogging;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using Serilog.Context;

namespace CM.Backend.Persistence.Repositories
{
	public abstract class MongoReadmodelRepository<TEntity> : IMongoReadmodelRepository<TEntity> where TEntity : IEntity
	{
		protected readonly ILogger Logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
		protected readonly IMongoDatabase DefaultDatabase;
		protected readonly string CollectionName = $"rm-{typeof(TEntity).Name.ToLower()}";
		protected IMongoCollection<TEntity> DefaultCollection =>
			DefaultDatabase.GetCollection<TEntity>(CollectionName);


		protected UpdateDefinitionBuilder<TEntity> Update => Builders<TEntity>.Update;
		protected SortDefinitionBuilder<TEntity> Sort => Builders<TEntity>.Sort;
		protected FilterDefinitionBuilder<TEntity> Filter => Builders<TEntity>.Filter;
		protected ProjectionDefinitionBuilder<TEntity> Projection => Builders<TEntity>.Projection;
		protected IndexKeysDefinitionBuilder<TEntity> IndexKeys => Builders<TEntity>.IndexKeys;

		public MongoReadmodelRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor)
		{
			Logger = logger;
			_httpContextAccessor = httpContextAccessor;
			DefaultDatabase = client.GetDatabase(config.Value.DefaultProjectionsDatabaseName);
				
			
			if (!CollectionExists(DefaultDatabase, CollectionName))
				DefaultDatabase.CreateCollection(CollectionName);
		}


		public async Task<bool> Delete(Guid id)
		{
			Logger.Information("Trying to delete {Entity} with {Id}", typeof(TEntity).Name, id);
			return (await DefaultCollection.DeleteOneAsync(Filter.Eq(x => x.Id, id)))
				.IsAcknowledged;
		}

		public async Task<IEnumerable<TEntity>> GetAll(int skip = 0, int limit = 100)
		{
			return await ExecuteCmd(
				() =>
					DefaultCollection
						.Find(Filter.Empty)
						.Skip(skip)
						.Limit(limit)
						.ToListAsync()
			, $"{typeof(TEntity).Name} - {nameof(GetAll)}");
		}

		public async Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
			Expression<Func<TEntity, TNewProjection>> projection, int skip = 0, int limit = 100)
		{
			return await ExecuteCmd(
				() =>
					DefaultCollection
						.Find(Filter.Empty)
						.Skip(skip)
						.Limit(limit)
						.Project(projection)
						.ToListAsync(),
				$"{typeof(TNewProjection).Name} - {nameof(GetAll)}");
		}
		
		public async Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
			Expression<Func<TEntity, TNewProjection>> projection,
			Expression<Func<TEntity, object>> sortDefiniton,
			int skip = 0,
			int limit = 100)
		{
			
			return await ExecuteCmd(
				() =>
					DefaultCollection
						.Find(Filter.Empty)
						.Skip(skip)
						.Limit(limit)
						.Project(projection)
						.SortByDescending(sortDefiniton)
						.ToListAsync(),
				$"{typeof(TNewProjection).Name} - {nameof(GetAll)}");
		}
		
		public async Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
			Expression<Func<TEntity, TNewProjection>> projection,
			Expression<Func<TEntity, object>> sortDefiniton,
			Expression<Func<TEntity, bool>> filter,
			int skip = 0,
			int limit = 100)
		{
			
			return await ExecuteCmd(
				() =>
					DefaultCollection
						.Find(filter)
						.Skip(skip)
						.Limit(limit)
						.Project(projection)
						.SortByDescending(sortDefiniton)
						.ToListAsync(),
				$"{typeof(TNewProjection).Name} - {nameof(GetAll)}");
		}
		
		public async Task<IEnumerable<TNewProjection>> GetAll<TNewProjection>(
			Expression<Func<TEntity, TNewProjection>> projection,
			Expression<Func<TEntity, bool>> filter,
			int skip = 0,
			int limit = 100)
		{
			
			return await ExecuteCmd(
				() =>
					DefaultCollection
						.Find(filter)
						.Skip(skip)
						.Limit(limit)
						.Project(projection)
						.ToListAsync(),
				$"{typeof(TNewProjection).Name} - {nameof(GetAll)}");
		}

		public async Task<TEntity> GetByIndex(int index, int collectionSize)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(Filter.Empty)
					.Skip(index)
					.Limit(1)
					.FirstOrDefaultAsync(),
				$"{nameof(GetByIndex)} - {nameof(MongoReadmodelRepository<TEntity>)}");

		}

        public async Task<IEnumerable<TEntity>> GetPaged(int page, int pageSize)
        {
	        return await ExecuteCmd(() =>
			        GetAll(page * pageSize, pageSize),
		        $"{nameof(GetPaged)} - {nameof(MongoReadmodelRepository<TEntity>)}");
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await ExecuteCmd(() =>
	            DefaultCollection.Find(b => b.Id == id).SingleOrDefaultAsync(),
	            $"{nameof(GetById)} - {nameof(MongoReadmodelRepository<TEntity>)}");
        }

        public async Task<Guid> Insert(TEntity entity)
        {
	        await ExecuteCmd(() =>
		        DefaultCollection.InsertOneAsync(entity, new InsertOneOptions()),
		        $"{nameof(Insert)} - {nameof(MongoReadmodelRepository<TEntity>)}");
	        
	        Logger.Information("Saved {@Entity}", entity);
            return entity.Id;
        }

        private bool CollectionExists(IMongoDatabase db, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collections = db.ListCollections(new ListCollectionsOptions { Filter = filter });
            return collections.Any();
        }

		protected async Task<TResult> ExecuteCmd<TResult>(Func<Task<TResult>> cmd, string cmdName = null)
		{
			var sw = new Stopwatch();
			sw.Start();

			var result = await cmd();
			sw.Stop();

			SharedLoggingProperties.AddMongoDbCommunicationInfo(_httpContextAccessor.HttpContext.TraceIdentifier, sw.ElapsedMilliseconds, cmdName);
			
			return result;
		}
		
		protected async Task ExecuteCmd(Func<Task> cmd, string cmdName = null)
		{
			var sw = new Stopwatch();
			sw.Start();

			await cmd();
			sw.Stop();

			SharedLoggingProperties.AddMongoDbCommunicationInfo(_httpContextAccessor.HttpContext.TraceIdentifier, sw.ElapsedMilliseconds, cmdName);
		}
    }
}