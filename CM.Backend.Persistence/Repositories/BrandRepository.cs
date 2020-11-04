using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using static CM.Backend.Persistence.Model.Brand;

namespace CM.Backend.Persistence.Repositories
{
    public interface IBrandRepository : IMongoReadmodelRepository<Brand>
    {
        Task AddChampagne(Guid brandId, Guid champagneId);//
		Task AddBrandPage(Guid brandId, Guid brandPageId);//
        Task<IEnumerable<Brand>> GetAllBrands(int skip = 0, int limit = 100); //
		Task RemoveBrandPage(Guid brandId, Guid brandPageId); //
		Task UpdatePublishingStatus(Guid brandId, bool publish); //
		Task<IEnumerable<Brand>> GetPagedWithFilterAsync(int page = 0, int pageSize = 100, bool includeUnPublished = false, bool sortAscending = true); //
	    Task UpdateBrandInfo(Guid brandId, string name, string brandProfileText); //
		Task UpdateBrandImages(Guid brandId, Guid brandCoverImageId, Guid brandListImageId, Guid bottleCoverImageId, Guid logoImageId);
	    Task UpdateBrandSocial(Guid brandId, Social social);
	    Task UpdateBrandCellar(Guid brandId, Guid coverImageId, Guid cardImageId);
		Task AddChampagneToPublished(Guid brandId, Guid champagneId);
	    Task RemoveChampagneFromPublished(Guid brandId, Guid champagneId);
	    Task AddCellarSection(Guid brandId, CellarSection section);
	    Task UpdateCellarSection(Guid brandId, Guid sectionId, CellarSection section);
	    Task DeleteCellarSection(Guid brandId, Guid sectionId);
	    Task<IEnumerable<BrandSearchProjectionModel>> SearchBrands(string searchText, int page, int pageSize, HashSet<Guid> brandIds = null);

	    Task<Dictionary<Guid, string>> GetBrandNameFromIds(HashSet<Guid> brandIds);
    }

    public class BrandRepository : MongoReadmodelRepository<Brand>, IBrandRepository
    {
	    public BrandRepository(IMongoClient client, IOptions<ProjectionsPersistenceConfiguration> config, ILogger logger, IHttpContextAccessor httpContextAccessor) : base(client, config, logger, httpContextAccessor)
	    {
	    }

	    public async Task UpdateBrandInfo(Guid brandId, string name, string brandProfileText)
	    {
		    await ExecuteCmd(() =>
				    DefaultCollection.FindOneAndUpdateAsync(b => b.Id == brandId, Update
					    .Set(b => b.Name, name)
					    .Set(b => b.BrandProfileText, brandProfileText)),
			    $"{nameof(UpdateBrandInfo)} - {nameof(BrandRepository)}");
	    }

	    public async Task UpdateBrandImages(Guid brandId, Guid brandCoverImageId, Guid brandListImageId, Guid bottleCoverImageId, Guid logoImageId)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.FindOneAndUpdateAsync(b => b.Id == brandId, Update
				    .Set(b => b.BrandCoverImgId, brandCoverImageId)
				    .Set(b => b.BrandListImgId, brandListImageId)
				    .Set(b => b.BottleCoverImgId, bottleCoverImageId)
				    .Set(b => b.LogoImgId, logoImageId)),
			    $"{nameof(UpdateBrandImages)} - {nameof(BrandRepository)}");
	    }

	    public async Task UpdateBrandSocial(Guid brandId, Social social)
	    {
		    await ExecuteCmd(() =>
				    DefaultCollection.FindOneAndUpdateAsync(b => b.Id == brandId, Update.Set(b => b.Social, social)),
		    $"{nameof(UpdateBrandSocial)} - {nameof(BrandRepository)}");
	    }

	    public async Task UpdateBrandCellar(Guid brandId, Guid coverImageId, Guid cardImageId)
	    {
		    var brand = await ExecuteCmd(() =>
			    DefaultCollection.Find(b => b.Id == brandId).SingleOrDefaultAsync(),
			    $"{nameof(UpdateBrandCellar)} - {nameof(BrandRepository)}");
		    
			//Update brand-cellar
		    var cellar = brand.Cellar;
		    cellar.CardImgId = cardImageId;
		    cellar.CoverImgId = coverImageId;

		    await ExecuteCmd(() =>
			    DefaultCollection.FindOneAndUpdateAsync(b => b.Id == brandId, Update.Set(b => b.Cellar, cellar)));
	    }
	    
	    
		public async Task AddBrandPage(Guid brandId, Guid brandPageId)
		{
			await ExecuteCmd(() =>
				DefaultCollection.FindOneAndUpdateAsync(b => b.Id == brandId,
					Update.AddToSet(b => b.BrandPageIds, brandPageId)),
				$"{nameof(AddBrandPage)} - {nameof(BrandRepository)}");
		}

        public async Task AddChampagne(Guid brandId, Guid champagneId)
        {
	        await ExecuteCmd(() =>
		        DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
			        Update.AddToSet(b => b.ChampagneIds, champagneId)),
		        $"{nameof(AddChampagne)} - {nameof(BrandRepository)}");
        }

	    public async Task AddChampagneToPublished(Guid brandId, Guid champagneId)
	    {
		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
				    Update.AddToSet(b => b.PublishedChampagneIds, champagneId)),
			    $"{nameof(AddChampagneToPublished)} - {nameof(BrandRepository)}");
	    }

	    public async Task RemoveChampagneFromPublished(Guid brandId, Guid champagneId)
	    {
		    /*var brand = await base.GetById(brandId);

		    var publishedChampagnes = brand.PublishedChampagneIds;
		    var newPublishedChampagnes = new List<Guid>();

		    foreach (Guid id in publishedChampagnes)
		    {
			    if (id != champagneId)
			    {
				    newPublishedChampagnes.Add(id);
			    }
		    }*/
		    await ExecuteCmd(() =>
				    DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
					    Update.Pull(b => b.PublishedChampagneIds, champagneId)),
			    $"{nameof(RemoveChampagneFromPublished)} - {nameof(BrandRepository)}");
	    }

	    public async Task AddCellarSection(Guid brandId, CellarSection section)
	    {
		    var brand = await ExecuteCmd(() =>
			    GetById(brandId),
			    $"{nameof(AddCellarSection)} - {nameof(BrandRepository)}");

		    var cellar = brand.Cellar;

		    cellar.Sections.Add(section);

		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
				    Update.Set(b => b.Cellar, cellar)),
			    $"{nameof(AddCellarSection)} - {nameof(BrandRepository)}");
	    }

	    public async Task UpdateCellarSection(Guid brandId, Guid sectionId, CellarSection section)
	    {
		    var brand = await ExecuteCmd(() =>
			    GetById(brandId));

		    var cellar = brand.Cellar;

		    foreach (var cellarSection in cellar.Sections)
		    {
			    if (cellarSection.Id.Equals(sectionId))
			    {
				    cellarSection.Title = section.Title;
				    cellarSection.Body = section.Body;
				    cellarSection.Champagnes = section.Champagnes;
			    }
		    }

		    await ExecuteCmd(() =>
			    DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
				    Update.Set(b => b.Cellar, cellar)),
			    $"{nameof(UpdateCellarSection)} - {nameof(BrandRepository)}");
	    }

	    public async Task DeleteCellarSection(Guid brandId, Guid sectionId)
	    {
		    var brand = await ExecuteCmd(() =>
				    GetById(brandId),
			    $"{nameof(DeleteCellarSection)} - {nameof(BrandRepository)}");

		    var cellar = brand.Cellar;

		    CellarSection toBeDeleted = null;
		    
		    foreach (var cellarSection in cellar.Sections)
		    {
			    if (cellarSection.Id.Equals(sectionId))
			    {
				    toBeDeleted = cellarSection;
			    }
		    }

		    cellar.Sections.Remove(toBeDeleted);

		    await ExecuteCmd(() =>
				    DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
					    Update.Set(b => b.Cellar, cellar)),
			    $"{nameof(DeleteCellarSection)} - {nameof(BrandRepository)}");
	    }

	    public async Task<IEnumerable<BrandSearchProjectionModel>> SearchBrands(string searchText, int page, int pageSize, HashSet<Guid> brandIds = null)
	    {
		    HashSet<Guid> set = brandIds;
		    if (brandIds == null)
		    {
			    set = new HashSet<Guid>();
		    }
		    
		    var f1 = new BsonDocument {{"name", new BsonDocument {{"$regex", searchText}, {"$options", "i"}}}};
		    var f2 = Filter.Where(c => set.Contains(c.Id));

		    return await ExecuteCmd(() =>
				    DefaultCollection
					    .Find(Filter.Or(f1, f2))
					    .Skip(page * pageSize)
					    .Limit(pageSize)
					    .Project(c => new BrandSearchProjectionModel
					    {
						    Id = c.Id,
						    ImageId = c.LogoImgId,
						    Name = c.Name
					    })
					    .ToListAsync(),
			    $"{nameof(SearchBrands)} - {nameof(BrandRepository)}");
	    }

	    public async Task<Dictionary<Guid, string>> GetBrandNameFromIds(HashSet<Guid> brandIds)
	    {
		    var result = await ExecuteCmd(() =>
				    DefaultCollection
					    .Find(x => brandIds.Contains(x.Id))
					    .Project(c => new KeyValuePair<Guid, string>(c.Id, c.Name))
					    .ToListAsync(),
			    $"{nameof(GetBrandNameFromIds)} - {nameof(BrandRepository)}");
		    
		    var dic = new Dictionary<Guid, string>();
		    
		    result.ForEach(x => dic.Add(x.Key, x.Value));

		    return dic;
	    }


	    public async Task<IEnumerable<Brand>> GetAllBrands(int skip = 0, int limit = 100)
		{
			return await ExecuteCmd(() =>
					GetPaged(skip, limit),
				$"{nameof(GetAllBrands)} - {nameof(BrandRepository)}");
        }

		public async Task<IEnumerable<Brand>> GetPagedWithFilterAsync(int page = 0, int pageSize = 100, bool includeUnpublished = false, bool SortAscending = true)
		{
			var filter = Filter.Empty;
			SortDefinition<Brand> sorting;

			//If unpublished brands shouldn't be included change the filter to only included published brands
			if (!includeUnpublished)
			{
				filter = Filter.Eq(f => f.Published, true);
			}

			//Decide if brand should be ordered ascending or descending alphabetically.
			if (SortAscending)
			{
				sorting = Sort.Ascending(b => b.Name);
			}
			else
			{
				sorting = Sort.Descending(b => b.Name);
			}

			return await ExecuteCmd(() =>
				DefaultCollection
					.Find(filter)
					.Skip(page * pageSize)
					.Limit(pageSize)
					.Sort(sorting)
					.ToListAsync()
			, $"{nameof(GetPagedWithFilterAsync)} - {nameof(BrandRepository)}");
		}

		public async Task RemoveBrandPage(Guid brandId, Guid brandPageId)
		{
			await ExecuteCmd(() =>
					DefaultCollection.UpdateOneAsync(b => b.Id == brandId,
						Update.Pull(x => x.BrandPageIds, brandPageId)),
				$"{nameof(RemoveBrandPage)} - {nameof(BrandRepository)}");
		}

		public async Task UpdatePublishingStatus(Guid brandId, bool publish)
		{
			await ExecuteCmd(() =>
					DefaultCollection.UpdateOneAsync(b => b.Id == brandId, Update.Set(a => a.Published, publish)),
				$"{nameof(UpdatePublishingStatus)} - {nameof(BrandRepository)}");
        }
	}
}