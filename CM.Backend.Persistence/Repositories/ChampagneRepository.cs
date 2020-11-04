using System.Threading.Tasks;
using System;
using System.Collections;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using Microsoft.Extensions.Options;

using MongoDB.Driver;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Transactions;
using CM.Backend.Persistence.Helpers;
using CM.Backend.Persistence.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Serilog;

namespace CM.Backend.Persistence.Repositories
{
    public interface IChampagneRepository : IMongoReadmodelRepository<Champagne>
    {
        Task AddChampagneProfile(Guid champagneId, ChampagneProfile champagneProfile);
        Task<IEnumerable<Champagne>> GetChampagnesFromList(List<Guid> champagneIds);
		Task SetChampagnePublishStatus(Guid champagneId, bool IsPublished);
		Task<IEnumerable<Champagne>> GetChampagnesByBrandId(Guid brandId);
		Task EditChampagne(Guid champagneId, string name, Guid bottleImgId, VintageInfo vintageInfo);
		Task EditChampagneProfile(Guid champagneId, ChampagneProfile champagneProfile);
		Task AddChampagneRating(Guid champagneId, Guid tastingId, double rating);
	    Task EditChampagneRating(Guid champagneId, Guid tastingId, double rating);
	    Task DeleteChampagneRating(Guid champagneId, Guid tastingId);
	    Task<IEnumerable<Champagne>> GetChampagneByIdFromListAsync(List<Guid> idList);
	    Task<long> CountDocuments();
	    Task<long> CountPublishedChampagnes();
	    Task<IEnumerable<Champagne>> GetChampagnesRandom(int numberOfRandomChampagnesRequested);
		Task<IEnumerable<Champagne>> GetChampagnesByFilterPagedAsync(bool isVintage, bool isNonVintage,
		    List<string> champagneStyle, List<string> champagneDosage, double lowerRating, double upperRating, int page,
		    int pageSize);
	    Task AddFolderDependencies(List<Guid> champagneIds, Guid champagneFolderId, string champagneFolderType);
	    Task AddFolderDependencies(Guid champagneId, Guid champagneFolderId, string champagneFolderType);
	    Task RemoveFolderDependencies(Guid champagneId, Guid champagneFolderId);
	    Task RemoveFolderDependencies(List<Guid> champagneIds, Guid champagneFolderId);
	    Task<IEnumerable<ChampagneSearchProjectionModel>> SearchChampagnes(string searchText, int page, int pageSize, HashSet<Guid> brandIds = null);

	    Task<IEnumerable<Champagne>> GetTop10(bool filterByVintage, bool filterByHighestRating, List<string> styles,
		    List<string> dosages, List<string> excludedStyles);

	    Task UpdateChampagneAverageRatingAndRatingCount(Guid champagneId, double averageRating, double ratingCount, double ratingValue);
	    Task SetChampagneRatingUpdateStatus(Guid champagneId, bool updateStatus);
    }

    public class ChampagneRepository : MongoReadmodelRepository<Champagne>, IChampagneRepository
    {
	    //var f1 = new BsonDocument {{"$text", new BsonDocument{{"$search", searchText}}}};
	    private readonly IMemoryCache _memoryCache;

	    public ChampagneRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache) : base(client, config, logger, httpContextAccessor)
	    {
		    _memoryCache = memoryCache;
		    var textIndex = IndexKeys.Text(c => c.BottleName);
		    var listIndex = IndexKeys.Ascending(c => c.BrandId);
		    DefaultCollection.Indexes.CreateOne(textIndex);
		    DefaultCollection.Indexes.CreateOne(listIndex);
		    
	    }

	    public async Task AddChampagneProfile(Guid champagneId, ChampagneProfile champagneProfile)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.FindOneAndUpdateAsync(c => c.Id == champagneId,
				    Update.Set(p => p.champagneProfile, champagneProfile)),
			    $"{nameof(AddChampagneProfile)} - {nameof(ChampagneRepository)}");
	    }

		public async Task EditChampagne(Guid champagneId, string name, Guid bottleImgId, VintageInfo vintageInfo)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(c => c.Id == champagneId, Update
					.Set(p => p.BottleName, name)
					.Set(p => p.BottleImgId, bottleImgId)
					.Set(p => p.vintageInfo, vintageInfo)),
				$"{nameof(EditChampagne)} - {nameof(ChampagneRepository)}");
		}

	    public async Task AddChampagneRating(Guid champagneId, Guid tastingId, double rating)
	    {
		    //Deprecated -- RatingCount, RatingValue and averageRating are now updated by the ratingRMEvenHandler
		    var champagne = await ExecuteCmd(() =>
			    GetById(champagneId),
			    $"{nameof(AddChampagneRating)} - {nameof(ChampagneRepository)}");

		    if (champagne.IsUpdated) //If the champagne.IsUpdated only update the ratingDictionary so that old code still works.
		    {
			    var ratingDicUpdate = champagne.RatingDictionary;
		    
			    ratingDicUpdate.Add(tastingId.ToString(), rating);
			    await ExecuteCmd(() =>
				    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId, Update
					    .Set(x => x.RatingDictionary, ratingDicUpdate)));
			    
			    return;
		    }
		    
		    var ratingDic = champagne.RatingDictionary;
		    
		    ratingDic.Add(tastingId.ToString(), rating);

		    var averageRating = 0.0;
		    var sum = 0.0;

		    foreach (var givenRating in ratingDic.Values)
		    {
			    sum += givenRating;
		    }

		    if (ratingDic.Count != 0)
		    {
			    averageRating = sum / ratingDic.Count;
		    }

		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId, Update
				    .Set(c => c.RatingDictionary, ratingDic)
				    .Set(c => c.AverageRating, averageRating)),
			    $"{nameof(AddChampagneRating)} - {nameof(ChampagneRepository)}");

	    }

	    public async Task EditChampagneRating(Guid champagneId, Guid tastingId, double rating)
	    {
		    var champagne = await ExecuteCmd(() =>
			    GetById(champagneId),
			    $"{nameof(EditChampagneRating)} - {nameof(ChampagneRepository)}");

		    if (champagne.IsUpdated)
		    {
			    var ratingDicUpdated = champagne.RatingDictionary;

			    if (ratingDicUpdated.ContainsKey(tastingId.ToString())) //Update rating for tastingId
			    {
				    ratingDicUpdated[tastingId.ToString()] = rating;
			    }
			    else
			    {
				    ratingDicUpdated.Add(tastingId.ToString(), rating);
			    }
			    
			    await ExecuteCmd(() =>
					    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId, Update
						    .Set(c => c.RatingDictionary, ratingDicUpdated)),
				    $"{nameof(EditChampagneRating)} - {nameof(ChampagneRepository)}");
			    
			    return;
		    }
		    
		    var ratingDic = champagne.RatingDictionary;

		    if (ratingDic.ContainsKey(tastingId.ToString())) //Update rating for tastingId
		    {
			    ratingDic[tastingId.ToString()] = rating;
		    }
		    else
		    {
			    ratingDic.Add(tastingId.ToString(), rating);
		    }

		    var averageRating = 0.0;
		    var sum = 0.0;

		    foreach (var givenRating in ratingDic.Values)
		    {
			    sum += givenRating;
		    }

		    if (ratingDic.Count != 0)
		    {
			    averageRating = sum / ratingDic.Count;
		    }

		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId, Update
				    .Set(c => c.RatingDictionary, ratingDic)
				    .Set(c => c.AverageRating, averageRating)),
			    $"{nameof(EditChampagneRating)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task DeleteChampagneRating(Guid champagneId, Guid tastingId)
	    {
		    var champagne = await ExecuteCmd(() =>
			    GetById(champagneId),
			    $"{nameof(DeleteChampagneRating)} - {nameof(ChampagneRepository)}");

		    if (champagne.IsUpdated)
		    {
			    var ratingDicUpdated = champagne.RatingDictionary;

			    ratingDicUpdated.Remove(tastingId.ToString());
			    
			    await ExecuteCmd(() =>
					    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId, Update
						    .Set(c => c.RatingDictionary, ratingDicUpdated)),
				    $"{nameof(DeleteChampagneRating)} - {nameof(ChampagneRepository)}");
			    
			    return;
		    }
		    
		    var ratingDic = champagne.RatingDictionary;

		    ratingDic.Remove(tastingId.ToString());

		    var averageRating = 0.0;
		    var sum = 0.0;

		    foreach (var givenRating in ratingDic.Values)
		    {
			    sum += givenRating;
		    }

		    if (ratingDic.Count != 0)
		    {
			    averageRating = sum / ratingDic.Count;
		    }

		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId, Update
				    .Set(c => c.RatingDictionary, ratingDic)
				    .Set(c => c.AverageRating, averageRating)),
			    $"{nameof(DeleteChampagneRating)} - {nameof(ChampagneRepository)}");
	    }

	    /// <summary>
	    /// Return an ordered list of all the champagnes decending by year, where there id is contained in idList.
	    /// </summary>
	    /// <param name="idList"></param>
	    /// <returns></returns>
	    public async Task<IEnumerable<Champagne>> GetChampagneByIdFromListAsync(List<Guid> idList)
	    {
		    return await ExecuteCmd(() =>
			    DefaultCollection
				    .Find(c => idList.Contains(c.Id))
				    .SortByDescending(c => c.vintageInfo.Year)
				    .ToListAsync(),
			    $"{nameof(GetChampagneByIdFromListAsync)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task<long> CountDocuments()
	    {
		    return await ExecuteCmd(() =>
			    DefaultCollection.CountAsync(new BsonDocument()),
			    $"{nameof(CountDocuments)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task<long> CountPublishedChampagnes()
	    {
		    return await ExecuteCmd(() =>
				    DefaultCollection.CountAsync(c => c.IsPublished),
			    $"{nameof(CountPublishedChampagnes)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task<IEnumerable<Champagne>> GetChampagnesRandom(int numberOfRandomChampagnesRequested)
	    {
			var publishedChampagnes = await ExecuteCmd(() =>
				DefaultCollection.Find(c => c.IsPublished).ToListAsync(),
				$"{nameof(GetChampagnesRandom)} - {nameof(ChampagneRepository)}");

		    var distinctRootIds = new List<Guid>();
		    var distinctChampagnes = new List<Champagne>();
		    
		    foreach (var champagne in publishedChampagnes)
		    {
			    Guid editionFolderId = Guid.Empty;
			    foreach (var folder in champagne.ChampagneFolderDependencies)
			    {
				    if (folder.Value.Equals("Editions"))
				    {
					    editionFolderId = Guid.Parse(folder.Key);
				    }
			    }

			    if (!editionFolderId.Equals(Guid.Empty))
			    {
				    if (!distinctRootIds.Contains(editionFolderId))
				    {
					    distinctRootIds.Add(editionFolderId);
					    distinctChampagnes.Add(champagne);
				    }
			    }
		    }
		    
		    var randomizer = new Random();
		    
			var possibleIndexes = Enumerable.Range(0, distinctChampagnes.Count).ToList();

			var chosenIndexes = new List<int>();

			for (int i = 0; i < numberOfRandomChampagnesRequested; i++)
			{
				if (i > distinctChampagnes.Count - 1)
				{
					break;
				}

				var index = randomizer.Next(0, possibleIndexes.Count);
				
				chosenIndexes.Add(possibleIndexes[index]);
				possibleIndexes.RemoveAt(index);
			}

		    var randomChampagne = new List<Champagne>();

		    foreach (var index in chosenIndexes)
		    {
			    randomChampagne.Add(distinctChampagnes[index]);
		    }

		    return randomChampagne;

	    }

	    public async Task<IEnumerable<Champagne>> GetChampagnesByFilterPagedAsync(bool isVintage, bool isNonVintage, List<string> champagneStyle, List<string> champagneDosage,
		    double lowerRating, double upperRating, int page, int pageSize)
	    {
		    var filterBuilder = Builders<Champagne>.Filter;

		    var filter = filterBuilder.AnyIn(x => x.champagneProfile.StyleCodes, champagneStyle);

		    if (isVintage && !isNonVintage)
		    {
			    Expression<Func<Champagne, bool>> p = c =>
				    c.IsPublished
					&& c.vintageInfo.IsVintage
					&& c.AverageRating >= lowerRating
				   	&& champagneDosage.Contains(c.champagneProfile.DisplayDosage);
			    
			    return await DefaultCollection
				    .Find(filterBuilder.And(p, filter))
				    .Skip(page * pageSize)
				    .Limit(pageSize)
				    .ToListAsync();
		    }
		    else if (!isVintage && isNonVintage)
		    {
			    Expression<Func<Champagne, bool>> p = c =>
				    c.IsPublished
				    && !c.vintageInfo.IsVintage
				    && c.AverageRating >= lowerRating
				    && champagneDosage.Contains(c.champagneProfile.DisplayDosage);
			    
			    return await DefaultCollection
				    .Find(filterBuilder.And(p, filter))
				    .Skip(page * pageSize)
				    .Limit(pageSize)
				    .ToListAsync();
		    }
		    else
		    {
			    Expression<Func<Champagne, bool>> p = c =>
				    c.IsPublished
				    && c.AverageRating >= lowerRating
				    && champagneDosage.Contains(c.champagneProfile.DisplayDosage);
			    
			    return await ExecuteCmd(() =>
				    DefaultCollection
					    .Find(filterBuilder.And(p, filter))
					    .Skip(page * pageSize)
					    .Limit(pageSize)
					    .ToListAsync(),
				    $"{nameof(GetChampagnesByFilterPagedAsync)} - {nameof(ChampagneRepository)}");
		    }
	    }

	    public async Task AddFolderDependencies(List<Guid> champagneIds, Guid champagneFolderId, string champagneFolderType)
	    {
		    var kvp = new KeyValuePair<string, string>(champagneFolderId.ToString(), champagneFolderType);
		    
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateManyAsync(c => champagneIds.Contains(c.Id), Update.AddToSet(c => c.ChampagneFolderDependencies, kvp)),
			    $"{nameof(AddFolderDependencies)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task AddFolderDependencies(Guid champagneId, Guid champagneFolderId, string champagneFolderType)
	    {
		    var kvp = new KeyValuePair<string, string>(champagneFolderId.ToString(), champagneFolderType);

		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId,
				    Update.AddToSet(c => c.ChampagneFolderDependencies, kvp)),
			    $"{nameof(AddFolderDependencies)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task RemoveFolderDependencies(Guid champagneId, Guid champagneFolderId)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(c => c.Id == champagneId,
				    Update.Pull(c => c.ChampagneFolderDependencies,
					    new KeyValuePair<string, string>(champagneFolderId.ToString(), "Editions"))),
			    $"{nameof(RemoveFolderDependencies)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task RemoveFolderDependencies(List<Guid> champagneIds, Guid champagneFolderId)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateManyAsync(c => champagneIds.Contains(c.Id),
				    Update.Pull(c => c.ChampagneFolderDependencies,
					    new KeyValuePair<string, string>(champagneFolderId.ToString(), "Editions"))),
			    $"{nameof(RemoveFolderDependencies)} - {nameof(ChampagneRepository)}");
	    }

	    public async Task<IEnumerable<ChampagneSearchProjectionModel>> SearchChampagnes(string searchText, int page, int pageSize, HashSet<Guid> brandIds = null)
	    {
		    const string CachedChampagneNames = "ChampagneNamesCached";
		    //At 625 bottles the total amount of cache is around 64000 bytes 64 KB... Nothing to worry about
		    //without cache this takes 193ms with pageSize=30 locally(longer on live environment)
		    //with cache it takes 0 ms even with linq going through 620 entities
		    var result = await _memoryCache.GetOrCreateAsync(CachedChampagneNames, async entry =>
		    {
			    entry.SetSlidingExpiration(TimeSpan.FromDays(1));

			    var champagnes = await ExecuteCmd(() =>
					    DefaultCollection
						    .Find(Filter.Empty)
						    .Project(x => new { Id = x.Id, Name = x.BottleName})
						    .ToListAsync(),
				    $"{nameof(SearchChampagnes)} - {nameof(ChampagneRepository)}");

			    Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
			    
			    foreach (var champagne in champagnes)
			    {
				    dic.Add(champagne.Id, RemoveDiacritics(champagne.Name));
			    }
			    
			    return dic;
		    });
		    
		    var textSearchMatches = result.Where(c => c.Value.Contains(searchText)).Select(x => x.Key).ToList();
		    //Ensure that the brandIds is never null. Even though it is okay not to provide it as parameter
		    var list = brandIds;
		    if (list == null)
		    {
			    list = new HashSet<Guid>();
		    }
			   
		    //Regex search filter, pattern matches
		    var f1 = new BsonDocument {{"bottleName", new BsonDocument {{"$regex", searchText}, {"$options", "i"}}}};
		    //Text search on bottleName using text index
		    var f2 = Filter.Where(c => textSearchMatches.Contains(c.Id));
		    //Search for all containing the brandId
		    var f3 = Filter.Where(c => list.Contains(c.BrandId));
		    
		    return await ExecuteCmd(() =>
				    DefaultCollection
					    .Find(Filter.Or(f1, f2, f3))
					    .Skip(page * pageSize)
					    .Limit(pageSize)
					    .Project(c => new ChampagneSearchProjectionModel
					    {
						    Id = c.Id,
						    BrandId = c.BrandId,
						    ImageId = c.BottleImgId,
						    IsVintage = c.vintageInfo.IsVintage,
						    Name = c.BottleName,
						    AverageRating = c.AverageRating,
						    Year = c.vintageInfo.Year.Value,
						    RatingDictionary = c.RatingDictionary
					    })
					    .ToListAsync(),
			    $"{nameof(SearchChampagnes)} - {nameof(ChampagneRepository)}"
		    );
	    }

	    public async Task<IEnumerable<Champagne>> GetTop10(bool filterByVintage, bool filterByHighestRating, List<string> styles, List<string> dosages, List<string> excludedStyles)
	    {
		    var filterBuilder = Builders<Champagne>.Filter;

		    var filters = new List<FilterDefinition<Champagne>>();
		    
		    //Default filters which applies to all searches.
		    var f1 = filterBuilder.AnyIn(x => x.champagneProfile.StyleCodes, styles);
		    
		    filters.Add(f1);

		    if (styles.Contains("TradBrut"))
		    {
			    var f2 = filterBuilder.Where(x => !x.champagneProfile.StyleCodes.Contains("Rose"));
			    filters.Add(f2);
		    }
		    
		    Expression<Func<Champagne, bool>> p = c =>
			    c.IsPublished
			    && c.vintageInfo.IsVintage == filterByVintage
			    && dosages.Contains(c.champagneProfile.DisplayDosage)
			    && c.RateCount > 4;
		    
		    filters.Add(p);

		    if (filterByHighestRating)
		    {
			    return await ExecuteCmd(() =>
				    DefaultCollection
					    .Find(filterBuilder.And(filters))
					    .Skip(0)
					    .Limit(10)
					    .SortByDescending(c => c.AverageRating)
					    .ToListAsync());
		    }
		    else
		    {
			    return await ExecuteCmd(() =>
				    DefaultCollection
					    .Find(filterBuilder.And(p, f1))
					    .Skip(0)
					    .Limit(10)
					    .SortByDescending(c => c.RateCount)
					    .ToListAsync());
		    }
	    }

	    public async Task UpdateChampagneAverageRatingAndRatingCount(Guid champagneId, double averageRating, double ratingCount, double ratingValue)
	    {
		    await DefaultCollection.UpdateOneAsync(x => x.Id.Equals(champagneId) && x.IsUpdated, Update
			    .Set(y => y.RateCount, ratingCount)
			    .Set(y => y.AverageRating, averageRating)
			    .Set(y => y.RateValue, ratingValue));
	    }

	    public async Task SetChampagneRatingUpdateStatus(Guid champagneId, bool updateStatus)
	    {
		    await DefaultCollection.UpdateOneAsync(x => x.Id.Equals(champagneId), Update.Set(y => y.IsUpdated, updateStatus));
	    }

	    private bool ContainsElementFromList(IEnumerable<string> list, List<string> containedIn)
	    {
		    var isContained = false;

		    foreach (var style in list)
		    {
			    if (containedIn.Contains(style))
			    {
				    isContained = true;
			    }
		    }

		    return isContained;
	    }

	    public async Task EditChampagneProfile(Guid champagneId, ChampagneProfile champagneProfile)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(c => c.Id == champagneId, Update
					.Set(p => p.champagneProfile, champagneProfile)),
				$"{nameof(EditChampagneProfile)} - {nameof(ChampagneRepository)}");
		}

		public async Task<IEnumerable<Champagne>> GetChampagnesByBrandId(Guid brandId)
		{
			return await ExecuteCmd(() =>
				DefaultCollection.Find(b => b.BrandId == brandId).ToListAsync(),
				$"{nameof(GetChampagnesByBrandId)} - {nameof(ChampagneRepository)}");
		}

		public async Task<IEnumerable<Champagne>> GetChampagnesFromList(List<Guid> champagneIds)
        {
            //We would like to make just one search on the mongoDb and make it match the ids inside champagneIds, getting all champagnes in one swoop
	        return await ExecuteCmd(() =>
		        DefaultCollection.Find(c => champagneIds.Contains(c.Id)).ToListAsync(),
		        $"{nameof(GetChampagnesFromList)} - {nameof(ChampagneRepository)}");
		}

		//Recursive selection sort => O(N^2) 
	    private List<Champagne> sort(List<Champagne> list)
	    {
		    return sort(list, 0, list.Count - 1);
	    }

        //Recursive selection sort helper method
	    private List<Champagne> sort(List<Champagne> list, int low, int high)
	    {
		    if (low < high)
		    {
			    int indexOfMin = low;

			    int yearOfMin = (int)list[low].vintageInfo.Year;
                Champagne swapChampagne = list[low];
			    
			    for (int i = low + 1; i <= high; i++)
			    {
				    if ((int) list[i].vintageInfo.Year < yearOfMin)
				    {
					    yearOfMin = (int) list[i].vintageInfo.Year;
					    swapChampagne = list[i];
					    indexOfMin = i;
				    }
			    }

			    list[indexOfMin] = list[low];

                list[low] = swapChampagne;
                
			    return sort(list, low + 1, high);
		    }

		    return list;
	    }

		public async Task SetChampagnePublishStatus(Guid champagneId, bool IsPublished)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(c => c.Id == champagneId,
					Update.Set(c => c.IsPublished, IsPublished)),
				$"{nameof(SetChampagnePublishStatus)} - {nameof(ChampagneRepository)}");
		}
	    
	    public static string RemoveDiacritics(string text) 
	    {
		    var normalizedString = text.Normalize(NormalizationForm.FormD);
		    var stringBuilder = new StringBuilder();
 
		    foreach (var c in normalizedString)
		    {
			    var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			    if (unicodeCategory != UnicodeCategory.NonSpacingMark)
			    {
				    stringBuilder.Append(c);
			    }
		    }
 
		    return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	    }
    }
}
