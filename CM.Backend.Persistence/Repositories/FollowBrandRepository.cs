using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    public interface IFollowBrandRepository : IMongoReadmodelRepository<FollowBrand>
    {
		Task DeleteFollow(FollowBrand.PrimaryKey key);
	    Task<FollowBrand> FindFollowByKey(FollowBrand.PrimaryKey key);
	    Task<IEnumerable<FollowBrand>> FindFollowByBrandId(Guid brandId);
	    Task<IEnumerable<FollowBrand>> GetFollowingFromUserId(Guid userId);
	    Task<int> CountFollowers(Guid brandId);
	    Task UpdateFollowByNameBatchAsync(Guid followById, string followByName);
	    Task UpdateFollowByProfileImageBatchAsync(Guid followById, Guid followByProfileImage);
	    Task UpdateBrandNameBatchAsync(Guid brandId, string brandName);
	    Task UpdateBrandLogoImageIdBatchAsync(Guid brandId, Guid brandImageLogoId);
	    Task<Dictionary<Guid, int>> CountFollowersForBrandIdsAsync(List<Guid> brandIds);
    }
    
    public class FollowBrandRepository : MongoReadmodelRepository<FollowBrand>, IFollowBrandRepository
    {
	    public FollowBrandRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
	    {
	    }

	    public async Task DeleteFollow(FollowBrand.PrimaryKey key)
		{
			var query = from e in DefaultCollection.AsQueryable()
						where e.Key.FollowByUserId == key.FollowByUserId
						where e.Key.FollowToBrandId == key.FollowToBrandId
						select e;

            foreach (var follow in query)
            {
                await ExecuteCmd(() =>
	                DefaultCollection.FindOneAndDeleteAsync(k => k.Id == follow.Id),
	                $"{nameof(DeleteFollow)} - {nameof(FollowBrandRepository)}");
            }
		}

	    public async Task<FollowBrand> FindFollowByKey(FollowBrand.PrimaryKey key)
	    {
		    return await ExecuteCmd(() =>
			    DefaultCollection.Find(k => k.Key == key).SingleOrDefaultAsync(),
			    $"{nameof(FindFollowByKey)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task<IEnumerable<FollowBrand>> FindFollowByBrandId(Guid brandId)
	    {
		    return await ExecuteCmd(() =>
			    DefaultCollection.Find(brand => brand.FollowToBrandId == brandId).ToListAsync(),
			    $"{nameof(FindFollowByBrandId)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task<IEnumerable<FollowBrand>> GetFollowingFromUserId(Guid userId)
	    {
		    return await ExecuteCmd(() =>
			    DefaultCollection.Find(f => f.FollowByUserId == userId).ToListAsync(),
			    $"{nameof(GetFollowingFromUserId)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task<int> CountFollowers(Guid brandId)
	    {
		    var count = await ExecuteCmd(() =>
				    DefaultCollection.CountDocumentsAsync(u => u.FollowToBrandId == brandId),
			    $"{nameof(CountFollowers)} - {nameof(FollowBrandRepository)}");
			  
		    return (int) count;
	    }

	    public async Task UpdateFollowByNameBatchAsync(Guid followById, string followByName)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateManyAsync(f => f.FollowByUserId == followById,
				    Update.Set(f => f.FollowByUserName, followByName)),
			    $"{nameof(UpdateFollowByNameBatchAsync)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task UpdateFollowByProfileImageBatchAsync(Guid followById, Guid followByProfileImage)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateManyAsync(f => f.FollowByUserId == followById,
				    Update.Set(f => f.FollowByUserProfileImgId, followByProfileImage)),
			    $"{nameof(UpdateFollowByProfileImageBatchAsync)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task UpdateBrandNameBatchAsync(Guid brandId, string brandName)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateManyAsync(f => f.FollowToBrandId == brandId,
				    Update.Set(f => f.FollowToBrandName, brandName)),
			    $"{nameof(UpdateBrandNameBatchAsync)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task UpdateBrandLogoImageIdBatchAsync(Guid brandId, Guid brandImageLogoId)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateManyAsync(f => f.FollowToBrandId == brandId,
				    Update.Set(f => f.FollowToBrandLogoImgId, brandImageLogoId)),
			    $"{nameof(UpdateBrandLogoImageIdBatchAsync)} - {nameof(FollowBrandRepository)}");
	    }

	    public async Task<Dictionary<Guid, int>> CountFollowersForBrandIdsAsync(List<Guid> brandIds)
	    {
		    var result = await ExecuteCmd(() =>
			    DefaultCollection.AsQueryable()
				    .Where(x => brandIds.Contains(x.FollowToBrandId))
				    .GroupBy(x => x.FollowToBrandId)
				    .Select(n => new {n.Key, Count = n.Count()})
				    .ToListAsync(),
			    $"{nameof(CountFollowersForBrandIdsAsync)} - {nameof(FollowBrandRepository)}");

		    return result.ToDictionary(x => x.Key, x => x.Count);
	    }
    }
}