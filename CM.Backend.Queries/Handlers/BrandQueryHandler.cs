using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.Entities;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Helpers;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using CM.Backend.Queries.Queries.BrandQueries;
using Microsoft.Extensions.Caching.Memory;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class BrandQueryHandler : 
    	IQueryHandler<GetBrand, BrandProfileExtendedBrandPage>, 
    	IQueryHandler<GetAllBrands, IEnumerable<BrandLight>>,
    	IQueryHandler<GetBrandLight, BrandLight>,
		IQueryHandler<GetCellar, Cellar>,
	    IQueryHandler<GetBrandFollowers, IEnumerable<FollowersQueryModel>>,
	    IQueryHandler<GetBrandCellarSection, CellarSection>,
	    IQueryHandler<GetBrandCellarSections, IEnumerable<CellarSection>>,
	    IQueryHandler<SearchBrands, IEnumerable<BrandSearchProjectionModel>>
    {
        private readonly IBrandRepository _repo;
        private readonly IBrandPageRepository brandPageRepository;
	    private readonly IFollowBrandRepository followBrandRepository;
	    private readonly IFollowRepository _followRepository;
	    private readonly ITastingRepository _tastingRepository;
	    private readonly IChampagneRepository _champagneRepository;
	    private readonly IChampagneFolderRepository _champagneFolderRepository;
	    private readonly IMemoryCache _memoryCache;

	    public BrandQueryHandler(IBrandRepository repo, IBrandPageRepository brandPageRepository, IFollowBrandRepository followBrandRepository, IFollowRepository followRepository, ITastingRepository tastingRepository, IChampagneRepository champagneRepository, IChampagneFolderRepository champagneFolderRepository, IMemoryCache memoryCache)
        {
            this.brandPageRepository = brandPageRepository;
	        this.followBrandRepository = followBrandRepository;
	        _followRepository = followRepository;
	        _tastingRepository = tastingRepository;
	        _champagneRepository = champagneRepository;
	        _champagneFolderRepository = champagneFolderRepository;
	        _memoryCache = memoryCache;
	        _repo = repo;
        }

        public async Task<BrandProfileExtendedBrandPage> HandleAsync(GetBrand query, CancellationToken ct)
        {
            //Get brand from persistence
            var brand = await _repo.GetById(query.BrandId);
	        
	        //Check if the user requesting the brandInformation is following this brand. If a record where followById = callerId && followToBrandId = brand.Id in the FollowBrand
	        //table then the user is following -> Return true else false
	        var isFollowing = await followBrandRepository.FindFollowByKey(
		        new FollowBrand.PrimaryKey {FollowByUserId = query.RequesterId, FollowToBrandId = query.BrandId});

            //Create BrandProfileExtended model which we will map into
            var brandProfileExtended = new BrandProfileExtendedBrandPage();

	        var editionCount = await _champagneFolderRepository.CountEditionFolderForBrandAsync(brand.Id);
	        
            //Foreach brandPageId get the brandPage and map into a brandPageLight model.
            foreach(var brandPageId in brand.BrandPageIds)
            {
                var brandPage = await brandPageRepository.GetById(brandPageId);
                var brandPageLight = new BrandPageLight();

                //Map
                brandPageLight.BrandPageId = brandPage.Id;
                brandPageLight.Title = brandPage.Title;
                brandPageLight.CardImgId = brandPage.CardImgId;
                brandPageLight.Url = brandPage.Url;
				brandPageLight.Labels = new List<string>();
                
				brandProfileExtended.Pages.Add(brandPageLight);
            }

	        var numberOfFollowers = await countFollowersForBrand(brand.Id);
	        var numberOfTastings = await countTastingsForBrand(brand.Id);
	        
			//Map brand cellar into brandProfileExtended cellarLight
			brandProfileExtended.Cellar = new CellarLight();
			brandProfileExtended.Cellar.Title = brand.Cellar.Title;
			brandProfileExtended.Cellar.CardImgId = brand.Cellar.CardImgId;
			brandProfileExtended.Cellar.URL = brand.Cellar.Url;
			brandProfileExtended.Cellar.Labels = new List<string>();
			brandProfileExtended.Cellar.Labels.Add(editionCount + " Bottles");
			brandProfileExtended.Cellar.Labels.Add(numberOfTastings + " Tastings");

            //Map from brand into brandProfileExtended
            brandProfileExtended.Id = brand.Id;
            brandProfileExtended.Published = brand.Published;
            brandProfileExtended.Name = brand.Name;
            brandProfileExtended.BrandProfileText = brand.BrandProfileText;
            brandProfileExtended.BrandCoverImageId = brand.BrandCoverImgId;
            brandProfileExtended.BottleCoverImageId = brand.BottleCoverImgId;
	        brandProfileExtended.BrandListImageId = brand.BrandListImgId;
	        brandProfileExtended.LogoImageId = brand.LogoImgId;
            brandProfileExtended.ChampagneIds = brand.PublishedChampagneIds;
	        brandProfileExtended.Social = brand.Social;
            brandProfileExtended.NumberOfTastings = numberOfTastings;
	        brandProfileExtended.NumberOfFollowers = numberOfFollowers;
	        brandProfileExtended.NumberOfEditions = editionCount;

	        //If isFollowing is null set follow to false else true
	        if (isFollowing != null)
	        {
		        brandProfileExtended.IsRequesterFollowing = true;
	        }
	        
            return brandProfileExtended;
        }

        public async Task<IEnumerable<BrandLight>> HandleAsync(GetAllBrands query, CancellationToken ct)
        {
	        //Fetch brands paged
            var result = await _repo.GetPagedWithFilterAsync(query.Page, query.PageSize, query.IncludeUnpublished, query.SortAscending);

	        var brandFollowers = new Dictionary<Guid, int>();
	        var tastingsForBrands = new Dictionary<Guid, int>();
	        var editionFoldersForBrands = new Dictionary<Guid, int>();
	        if (result.Any())
	        {
		        var brandIds = new List<Guid>(result.Select(x => x.Id));
		        //Count followers
		        var t1 = followBrandRepository
			        .CountFollowersForBrandIdsAsync(brandIds);
				//Count tastings
		        var t2 = _tastingRepository
			        .CountTastingsForBrandIdsAsync(brandIds);
				//Count editionFolders - Number of different champagnes
		        var t3 = _champagneFolderRepository
			        .CountEditionFoldersForBrandIdsAsync(brandIds);

		        //Start three concurrent threads and await them all here...
		        await Task.WhenAll(t1, t2, t3);

		        brandFollowers = t1.Result;
		        tastingsForBrands = t2.Result;
		        editionFoldersForBrands = t3.Result;
	        }
	        
	        var convertedResult = new List<BrandLight>();

	        //Map into convertedResult.
	        foreach (var brand in result)
	        {
		        var numberOfEditions = 0;
		        if(editionFoldersForBrands.ContainsKey(brand.Id))
					numberOfEditions = editionFoldersForBrands[brand.Id];

		        var numberOfFollowers = 0;
		        if (brandFollowers.ContainsKey(brand.Id))
			        numberOfFollowers = brandFollowers[brand.Id];

		        var numberOfTastings = 0;
		        if (tastingsForBrands.ContainsKey(brand.Id))
			        numberOfTastings = tastingsForBrands[brand.Id];
		        
		        convertedResult.Add(MappingResources.MapBrandToBrandLight(brand, numberOfFollowers, numberOfTastings, numberOfEditions));
	        }
	        
            return convertedResult;
        }
        
		public async Task<BrandLight> HandleAsync(GetBrandLight query, CancellationToken ct)
		{
			var result = await _repo.GetById(query.BrandId);
			var numberOfFollowers = await countFollowersForBrand(result.Id);
			var numberOfTastings = await countTastingsForBrand(result.Id);

			var convertedResult = new BrandLight
			{
				Id = result.Id,
				Name = result.Name,
				BrandCoverImageId = result.BrandCoverImgId,
				BottleCoverImageId = result.BottleCoverImgId,
				BrandLogoImageId = result.LogoImgId,
				ChampagneIds = result.PublishedChampagneIds,
				NumberOfTastings = numberOfTastings,
				NumberOfFollowers = numberOfFollowers
			};

			return convertedResult;
		}

		public async Task<Cellar> HandleAsync(GetCellar query, CancellationToken ct)
		{
			var brand = await _repo.GetById(query.BrandId);
			var brandRoots = await _champagneFolderRepository.GetFoldersByBrandId(query.BrandId);

			var viableRoots = new List<ChampagneFolderQueryModel>();
			var missingRoots = new List<Guid>();

			foreach (var root in brandRoots)
			{
				var convertedRoot = await ConvertChampagneRoot(root);

				if (convertedRoot != null)
				{
					viableRoots.Add(convertedRoot);
				}
			}
			
			if (brand.Cellar == null)
            {
                return null;
            }
           
            //New cellar we can map into
			var cellar = new Cellar();

			//Map brandCellar into cellar.
            cellar.Title = brand.Cellar.Title;
            cellar.CardImgId = brand.Cellar.CardImgId;
            cellar.HeaderImgId = brand.Cellar.CoverImgId;
            cellar.Url = brand.Cellar.Url;
			cellar.Sections = new List<Cellar.Section>();
			
			
            //Check if there is any sections inside brand.cellar. If so check if any champagnes are missing and add those as an additional section
			foreach(var section in brand.Cellar.Sections)
			{
				var cellarSection = new Cellar.Section();
				cellarSection.Title = section.Title;
				cellarSection.Body = section.Body;
				cellarSection.Champagnes = new List<ChampagneFolderQueryModel>();
				foreach (var viableRoot in viableRoots)
				{
					if (section.Champagnes.Contains(viableRoot.Id))
					{
						cellarSection.Champagnes.Add(viableRoot);
						missingRoots.Add(viableRoot.Id);
					}
				}

				if (cellarSection.Champagnes.Count > 0)
				{
					cellar.Sections.Add(cellarSection);
				}
			}

            //If there is any champagnes missing we should create new cellarSection with only champagnes
			if(missingRoots.Count != viableRoots.Count)
			{
				var missingSection = new Cellar.Section();
				missingSection.Title = "Cuvées";
				missingSection.Body = null;
				missingSection.Champagnes = new List<ChampagneFolderQueryModel>();
				foreach (var root in viableRoots)
				{
					if (!missingRoots.Contains(root.Id))
					{
						missingSection.Champagnes.Add(root);
					}
				}
				cellar.Sections.Add(missingSection);
			}

			return cellar;

		}

	    public async Task<IEnumerable<FollowersQueryModel>> HandleAsync(GetBrandFollowers query, CancellationToken ct)
	    {
		    //First get all entities in followBrandRepository where followToBrandId = query.BrandId
		    var followers = await followBrandRepository.FindFollowByBrandId(query.BrandId);

		    var followersList = new List<FollowBrand>(followers);
		    
		    //Since there were no followers return empty list
		    if (followers.Count() < 1)
		    {
			    return new List<FollowersQueryModel>();
		    }

		    var pagedList = new List<FollowBrand>();
		    //Only return entities that matches the range specified in the query.
		    for (int i = (query.Page * query.PageSize); i < followersList.Count(); i++)
		    {
			    if(i < (query.Page * query.PageSize) + query.PageSize)
			    {
				    pagedList.Add(followersList[i]);
			    }
		    }

		    return await ConvertBrandFollowers(pagedList, query.RequesterId);
		    
	    }

	    private async Task<IEnumerable<FollowersQueryModel>> ConvertBrandFollowers(IEnumerable<FollowBrand> brandFollowers, Guid requesterId)
	    {
		    var convertedList = new List<FollowersQueryModel>();
		    //If the requester is empty just map into FollowersQueryModel and return list
		    if (requesterId == Guid.Empty)
		    {
			    //Mapping
			    foreach(var followBrand in brandFollowers)
			    {
				    var followersQueryModel = new FollowersQueryModel
				    {
						Id = followBrand.Id,
					    FollowById =  followBrand.FollowByUserId,
					    FollowByName = followBrand.FollowByUserName,
					    FollowByProfileImgId = followBrand.FollowByUserProfileImgId,
					    IsRequesterFollowing = false
				    };
				    convertedList.Add(followersQueryModel);
			    }
		    }
		    else
		    {
			    //Foreach brandFollower, check if the requester is following them or not
			    foreach (var followBrand in brandFollowers)
			    {
				    var requesterFollowingEntity =
					    await _followRepository.GetFollowByKey(new Follow.PrimaryKey
					    {
						    FollowById = requesterId,
						    FollowToId = followBrand.FollowByUserId
					    });
				    //OBJECT TO MAP INTO
				    var followersQueryModel = new FollowersQueryModel();

				    followersQueryModel.Id = followBrand.Id;
				    followersQueryModel.FollowById = followBrand.FollowByUserId;
				    followersQueryModel.FollowByName = followBrand.FollowByUserName;
				    followersQueryModel.FollowByProfileImgId = followBrand.FollowByUserProfileImgId;
				    if (requesterFollowingEntity != null)
				    {
					    followersQueryModel.IsRequesterFollowing = true;
				    }
				    else
				    {
					    followersQueryModel.IsRequesterFollowing = false;
				    }

				    convertedList.Add(followersQueryModel);
			    }
		    }

		    return convertedList; 
	    }

	    private async Task<int> countFollowersForBrand(Guid brandId)
	    {
		    return await followBrandRepository.CountFollowers(brandId);
	    }

	    private async Task<int> countTastingsForBrand(Guid brandId)
	    {
		    return await _tastingRepository.CountTastingsForBrand(brandId);
	    }

	    public async Task<CellarSection> HandleAsync(GetBrandCellarSection query, CancellationToken ct)
	    {
		    var brand = await _repo.GetById(query.BrandId);

		    if (brand != null)
		    {
			    foreach (var section in brand.Cellar.Sections)
			    {
				    if (section.Id.Equals(query.SectionId))
				    {
					    return section;
				    }
			    }
		    }

		    return null;
	    }

	    public async Task<IEnumerable<CellarSection>> HandleAsync(GetBrandCellarSections query, CancellationToken ct)
	    {
		    var brand = await _repo.GetById(query.BrandId);

		    return brand.Cellar.Sections;

	    }
	    
	    private async Task<ChampagneFolderQueryModel> ConvertChampagneRoot(ChampagneFolder folder, bool isEmptyFoldersIncluded = false)
		{
			var champagneList = new List<Guid>();
			champagneList.AddRange(folder.ChampagneIds);

			//Fetch all champagnes for respective root
			var champagnes = await _champagneRepository.GetChampagnesFromList(champagneList);

			//Hold information about only the published champagnes
			var publishedChampagnes = new List<Persistence.Model.Champagne>();
			var publishedChampagnesId = new List<Guid>();

			double numberOfTastings = 0;
			double averageRatingSumOfTastings = 0;
			int numberOfBottlesWithTasting = 0;

			//Iterate over each champagne and check if they are published and if so include them, if not 
			foreach (var champagne in champagnes)
			{
				if (champagne.IsPublished)
				{
					//publishedChampagnes.Add(champagne);
					publishedChampagnes.Add(champagne);

					numberOfTastings += champagne.RateCount;
					averageRatingSumOfTastings += champagne.AverageRating;
				}
			}

			//Map from sorted list of champagnes to corresponding sorted list of champagneId
			foreach (var champagne in publishedChampagnes)
			{
				publishedChampagnesId.Add(champagne.Id);
				if (champagne.AverageRating > 0.0)
				{
					numberOfBottlesWithTasting++;
				}
			}

			//Fetch brandName
			var brand = await _repo.GetById(folder.AuthorId);

			//If isEmptyRootsIncluded is false we should return null if 
			if (!isEmptyFoldersIncluded)
			{
				if (publishedChampagnes.Count == 0)
				{
					return null;
				}
			}

			var queryFolder = new ChampagneFolderQueryModel();
			queryFolder.Id = folder.Id;
			queryFolder.FolderName = folder.FolderName;
			queryFolder.AuthorId = folder.AuthorId;
			queryFolder.AuthorName = brand.Name;
			queryFolder.DisplayImageId = folder.DisplayImageId;
			queryFolder.ChampagneIds = publishedChampagnesId.ToArray();
			queryFolder.FolderContentType = folder.ContentType;
			queryFolder.FolderType = folder.FolderType;
			if (averageRatingSumOfTastings == 0 || numberOfTastings == 0)
			{
				queryFolder.AverageRating = 0;
			}
			else
			{
				queryFolder.AverageRating = averageRatingSumOfTastings / numberOfBottlesWithTasting;
			}

			queryFolder.SumOfRating = 0.0;
			queryFolder.NumberOfTasting = numberOfTastings;

			return queryFolder;
		}

	    public async Task<IEnumerable<BrandSearchProjectionModel>> HandleAsync(SearchBrands query, CancellationToken ct)
	    {
		    const string CachedBrands = "GetAllBrandsCached";//TODO: All these cached keys should be in a global resource file!!!
		    
		    //Retreive all brands, there is only 50 so it will be okay for now, especially seeing as we are projecting on GetAll. 50 Brands * 3 properties = 20KB at most...
		    var brands = await _memoryCache.GetOrCreateAsync(CachedBrands, async entry =>
		    {
			    entry.SetSlidingExpiration(TimeSpan.FromDays(5));

			    return await _repo.GetAll(c => new BrandSearchProjectionModel
			    {
				    Id = c.Id,
				    ImageId = c.LogoImgId,
				    Name = c.Name
			    });
		    });

		    //We are not returning this because we might just as well use the repo so that it is already implemented for relevance searches
		    var diacriticSearch = DiacriticSearch(brands, query.SearchText).Select(c => c.Id);

		    return await _repo.SearchBrands(query.SearchText, query.Page, query.PageSize,
			    new HashSet<Guid>(diacriticSearch));
	    }

	    private HashSet<BrandSearchProjectionModel> DiacriticSearch(IEnumerable<BrandSearchProjectionModel> brands, string searchText)
	    {
		    var set = new HashSet<BrandSearchProjectionModel>();
		    
		    foreach (var key in brands)
		    {
			    if (MappingResources.RemoveDiacritics(key.Name.ToLower()).Contains(searchText.ToLower()))
			    {
				    set.Add(key);
			    }
		    }
		    
		    return set;
	    }
    }
}