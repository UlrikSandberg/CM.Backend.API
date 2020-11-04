using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
	public interface IChampagneFolderRepository : IMongoReadmodelRepository<ChampagneFolder>
	{
		Task AddChampagneToFolder(Guid id, Guid champagneId);
		Task<IEnumerable<ChampagneFolder>> GetFoldersByBrandId(Guid brandId);
		Task RemoveChampagneFromFolder(Guid id, Guid champagneId);
		Task EditChampagneFolder(Guid id, string folderName, Guid displayImageId, string contentType, bool isOnDiscover);
		Task<long> count();
		Task<IEnumerable<ChampagneFolder>> GetFoldersFromListAsync(List<Guid> champagneFolderIdList);
		Task<IEnumerable<ChampagneFolder>> GetPaged(int page, int pageSize, string folderType);
		Task<int> CountEditionFolderForBrandAsync(Guid brandId);
		Task<Dictionary<Guid, int>> CountEditionFoldersForBrandIdsAsync(List<Guid> brandIds);

		Task SetDiscoverVisibilityForIds(HashSet<Guid> ids, bool isVisible);
	}

	public class ChampagneFolderRepository : MongoReadmodelRepository<ChampagneFolder>, IChampagneFolderRepository
	{
		public ChampagneFolderRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
		{
		}

		public async Task AddChampagneToFolder(Guid id, Guid champagneId)
		{
			await ExecuteCmd(() =>
					DefaultCollection.UpdateOneAsync(r => r.Id == id,
						Update.AddToSet(c => c.ChampagneIds, champagneId)),
				$"{nameof(AddChampagneToFolder)} - {nameof(ChampagneFolderRepository)}");
		}


		public async Task EditChampagneFolder(Guid id, string folderName, Guid displayImageId, string contentType, bool isOnDiscover)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(r => r.Id == id, Update
					.Set(r => r.FolderName, folderName)
					.Set(r => r.DisplayImageId, displayImageId)
					.Set(r => r.ContentType, contentType)
					.Set(r => r.IsOnDiscover, isOnDiscover)),
				$"{nameof(EditChampagneFolder)} - {nameof(ChampagneFolderRepository)}");     
		}

		public async Task<long> count()
		{
			return await ExecuteCmd(() =>
				DefaultCollection.CountAsync(new BsonDocument()),
				$"{nameof(count)} - {nameof(ChampagneFolderRepository)}");
		}

		public async Task<IEnumerable<ChampagneFolder>> GetFoldersFromListAsync(List<Guid> champagneFolderIdList)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(c => champagneFolderIdList.Contains(c.Id)).ToListAsync(),
				$"{nameof(GetFoldersFromListAsync)} - {nameof(ChampagneFolderRepository)}");
		}

		public async Task<IEnumerable<ChampagneFolder>> GetPaged(int page, int pageSize, string folderType)
		{
			var sortingOption = GetDailyDiscoverSortingOption();
			
			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(c => c.FolderType.Equals(folderType) && c.IsOnDiscover)
					.Sort(sortingOption)
					.Skip(page * pageSize)
					.Limit(pageSize)
					.ToListAsync(),
				$"{nameof(GetPaged)} - {nameof(ChampagneFolderRepository)}");
		}

		public async Task<int> CountEditionFolderForBrandAsync(Guid brandId)
		{
			var result1 = await ExecuteCmd(() =>
					DefaultCollection.CountDocumentsAsync(x =>
						x.AuthorId.Equals(brandId) && x.FolderType.Equals("Editions")),
				$"{nameof(CountEditionFolderForBrandAsync)} - {nameof(ChampagneFolderRepository)}");

			return (int) result1;
		}

		public async Task<Dictionary<Guid, int>> CountEditionFoldersForBrandIdsAsync(List<Guid> brandIds)
		{
			var result = await ExecuteCmd(() =>
					DefaultCollection.AsQueryable()
						.Where(x => brandIds.Contains(x.AuthorId) && x.FolderType.Equals("Editions"))
						.GroupBy(x => x.AuthorId)
						.Select(n => new {n.Key, Count = n.Count()})
						.ToListAsync(),
				$"{nameof(CountEditionFoldersForBrandIdsAsync)} - {nameof(ChampagneFolderRepository)}");

			//Maps to a dictionary
			return result.ToDictionary(x => x.Key, x => x.Count); 
		}

		public async Task SetDiscoverVisibilityForIds(HashSet<Guid> ids, bool isVisible)
		{
			await ExecuteCmd(() =>
					DefaultCollection.UpdateManyAsync(x => ids.Contains(x.Id),
						Update.Set(y => y.IsOnDiscover, isVisible)),
				$"{nameof(SetDiscoverVisibilityForIds)} - {nameof(ChampagneFolderRepository)}");
		}

		public async Task<IEnumerable<ChampagneFolder>> GetFoldersByBrandId(Guid brandId)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(champagneRoot => champagneRoot.AuthorId == brandId)
					.ToListAsync(),
				$"{nameof(GetFoldersByBrandId)} - {nameof(ChampagneFolderRepository)}");
		}

		public async Task RemoveChampagneFromFolder(Guid id, Guid champagneId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.UpdateOneAsync(r => r.Id == id, Update.Pull(x => x.ChampagneIds, champagneId)),
				$"{nameof(RemoveChampagneFromFolder)} - {nameof(ChampagneFolderRepository)}");
		}

		private SortDefinition<ChampagneFolder> GetDailyDiscoverSortingOption()
		{
			var today = DateTime.UtcNow.DayOfWeek;
			
			switch (today)
			{
				case DayOfWeek.Monday:
					return Sort.Ascending(c => c.Id);
				case DayOfWeek.Tuesday:
					return Sort.Descending(c => c.Id);
				case DayOfWeek.Wednesday:
					return Sort.Ascending(c => c.DisplayImageId);
				case DayOfWeek.Thursday:
					return Sort.Descending(c => c.DisplayImageId);
				case DayOfWeek.Friday:
					return Sort.Ascending(c => c.Id);
				case DayOfWeek.Saturday:
					return Sort.Descending(c => c.Id);
				case DayOfWeek.Sunday:
					return Sort.Ascending(c => c.DisplayImageId);
				default:
					return null;
			}
		}
	}
}
