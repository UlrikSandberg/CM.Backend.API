using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CM.Backend.Documents.StaticResources;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using CM.Backend.Queries.Helpers;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.ChampagneModels;
using CM.Backend.Queries.Model.TastingModels;
using CM.Backend.Queries.Queries;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver.Core.Operations;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Handlers
{
    public class ChampagneQueryHandler : 
	IQueryHandler<GetChampagne, ChampagneQueryModel>,
	IQueryHandler<GetChampagnesInFolder, IEnumerable<ChampagneLight>>,
	IQueryHandler<GetChampagnes, IEnumerable<Champagne>>,
	IQueryHandler<GetBrandChampagnes, IEnumerable<Champagne>>,
	IQueryHandler<GetChampagneWithRatingAndTasting, ChampagneWithRatingAndTasting>,
	IQueryHandler<GetChampagnesByFilterPagedAsync, IEnumerable<ChampagneLight>>,
	IQueryHandler<SearchChampagnes, IEnumerable<ChampagneSearchModel>>
    {
        private readonly IChampagneRepository champagneRepository;
	    private readonly IUserRepository userRepository;
	    private readonly IChampagneFolderRepository champagneFolderRepository;
	    private readonly IBrandRepository brandRepository;
	    private readonly ITastingRepository tastingRepository;
	    private readonly ICommentRepository commentRepository;
	    private readonly ILikeRepository likeRepository;
	    private readonly IMemoryCache _memoryCache;
	    private readonly IRatingRepository _ratingRepository;

	    public ChampagneQueryHandler(IUserRepository userRepository, IChampagneRepository champagneRepository, IChampagneFolderRepository champagneFolderRepository, IBrandRepository brandRepository, ITastingRepository tastingRepository, ICommentRepository commentRepository, ILikeRepository likeRepository, IMemoryCache memoryCache, IRatingRepository ratingRepository)
		{
			this.userRepository = userRepository;
			this.champagneFolderRepository = champagneFolderRepository;
			this.brandRepository = brandRepository;
			this.tastingRepository = tastingRepository;
			this.commentRepository = commentRepository;
			this.likeRepository = likeRepository;
			_memoryCache = memoryCache;
			_ratingRepository = ratingRepository;
			this.champagneRepository = champagneRepository;
		}

		public async Task<ChampagneQueryModel> HandleAsync(GetChampagne query, CancellationToken ct)
		{
			//***** Fetch entities such as champagne, champagneRoot and brand in order to make a more complex champagneQueryModel
			var champagne = await champagneRepository.GetById(query.ChampagneId);
			
			//Look for this champagnes edition folder.
			Guid editionFolderId = Guid.Empty;

			foreach (var folder in champagne.ChampagneFolderDependencies)
			{
				if (folder.Value.Equals("Editions"))
				{
					editionFolderId = Guid.Parse(folder.Key);
				}
			}

			ChampagneFolder editionFolder = null;
			if (!editionFolderId.Equals(Guid.Empty))
			{
				editionFolder = await champagneFolderRepository.GetById(editionFolderId);
			}

			var brand = await brandRepository.GetById(champagne.BrandId);
			
			//Map into champagneQueryModel;
			var champagneQueryModel = new ChampagneQueryModel();

			//***** Clean Mapping *****
			champagneQueryModel.Id = champagne.Id;
			champagneQueryModel.BottleName = champagne.BottleName;
			champagneQueryModel.BrandId = champagne.BrandId;
			champagneQueryModel.BrandName = brand.Name;
			champagneQueryModel.BrandProfileText = brand.BrandProfileText;
			champagneQueryModel.BottleCoverImgId = brand.BottleCoverImgId;
			champagneQueryModel.BrandCoverImgId = brand.BrandCoverImgId;
			champagneQueryModel.BottleImgId = champagne.BottleImgId;
			champagneQueryModel.IsPublished = champagne.IsPublished;
			champagneQueryModel.ChampagneRootId = editionFolderId;
			
			//***** Map -> Decide if the respective root only have one branch(champagne) *****
			if (editionFolder == null)
			{
				champagneQueryModel.RootIsSingleton = true;
			}
			else
			{
				if (editionFolder.ChampagneIds.Length > 1)
				{
					champagneQueryModel.RootIsSingleton = false;
				}
				else
				{
					champagneQueryModel.RootIsSingleton = true;
				}
			}
			//***** Map Vintage info *****
			champagneQueryModel.vintageInfo = new ChampagneQueryModel.VintageInfo
			{
				IsVintage = champagne.vintageInfo.IsVintage,
				Year = champagne.vintageInfo.Year
			};
			
			//***** Map champagneProfile *****
			if (champagne.champagneProfile != null)
			{
				champagneQueryModel.champagneProfile = new ChampagneQueryModel.ChampagneProfile
				{
					AgeingPotential = champagne.champagneProfile.AgeingPotential,
					AlchoholVol = champagne.champagneProfile.AlchoholVol,
					Appearance = champagne.champagneProfile.Appearance,
					Aroma = champagne.champagneProfile.Aroma,
					BlendInfo = champagne.champagneProfile.BlendInfo,
					BottleHistory = champagne.champagneProfile.BottleHistory,
					Dosage = champagne.champagneProfile.DisplayDosage,
					Style = champagne.champagneProfile.DisplayStyle,
					Character = champagne.champagneProfile.DisplayCharacter,
					Chardonnay = champagne.champagneProfile.Chardonnay,
					DosageAmount = champagne.champagneProfile.DosageAmount,
					FoodPairing = champagne.champagneProfile.FoodPairing,
					PinotMeunier = champagne.champagneProfile.PinotMeunier,
					PinotNoir = champagne.champagneProfile.PinotNoir,
					RedWineAmount = champagne.champagneProfile.RedWineAmount,
					Taste = champagne.champagneProfile.Taste,
					ServingTemp = champagne.champagneProfile.ServingTemp,
					ReserveWineAmount = champagne.champagneProfile.ReserveWineAmount
				};
			}

			//***** Map filterSearchParameters *****
			if (champagne.champagneProfile != null)
			{
				champagneQueryModel.filterSearchParameters = new ChampagneQueryModel.FilterSearchParameters
				{
					Dosage = champagne.champagneProfile.DisplayDosage,
					Style = champagne.champagneProfile.StyleCodes,
					Character = champagne.champagneProfile.CharacterCodes
				};
			}

			//***** Check if the user requesting this bottle have bookmarked and tasted this bottle
			if (query.ReqUserId == Guid.Empty)
			{
				champagneQueryModel.IsBookmarkedByRequester = false;
				champagneQueryModel.IsTastedByRequester = false;
			}
			else
			{
				var champagneIsBookmarked = await userRepository.ChampagneIsBookmarkedByUser(query.ReqUserId, query.ChampagneId);
				var champagneIsTasted = await userRepository.ChampagneIsTastedByUser(query.ReqUserId, query.ChampagneId);
				if (!champagneIsBookmarked)
					champagneQueryModel.IsBookmarkedByRequester = false;
				else
					champagneQueryModel.IsBookmarkedByRequester = true;

				if (!champagneIsTasted)
					champagneQueryModel.IsTastedByRequester = false;
				else
					champagneQueryModel.IsTastedByRequester = true;

				if (champagneQueryModel.IsTastedByRequester)
				{
					champagneQueryModel.RequesterTasting = await tastingRepository.GetTasting(query.ReqUserId, query.ChampagneId);
				}
			}
			
			//***** Map ratings *****
			champagneQueryModel.NumberOfTastings = (int)champagne.RateCount;
			champagneQueryModel.RatingSumOfTastings = champagne.RateValue; //MappingResources.CalculateRatingSum(champagne.RatingDictionary);
			champagneQueryModel.AverageRating = champagne.AverageRating; //MappingResources.CalculateAverageRating(champagne.RatingDictionary);
			
			//All mapping done! Return the new updated model
			return champagneQueryModel;

		}

		/// <summary>
		/// Return a list of all the editions/vintage champagnes insides this champagneFolder sorted decending on vintage year
		/// </summary>
		/// <param name="query"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public async Task<IEnumerable<ChampagneLight>> HandleAsync(GetChampagnesInFolder query, CancellationToken ct)
		{
			//Retrieve respective champagneFolder based on Id, and the retrieve the respective brand. 
			var root = await champagneFolderRepository.GetById(query.ChampagneFolderId);

			//In-memory cache of brand names!
			var brand = await _memoryCache.GetOrCreateAsync(root.AuthorId, async entry =>
			{
				entry.SetSlidingExpiration(TimeSpan.FromDays(1));
				return await brandRepository.GetById(root.AuthorId);
			});
			     
			//Query mongoDB for all folder content sorted decending on year.
			var champagnesInFolder = await champagneRepository.GetChampagneByIdFromListAsync(new List<Guid>(root.ChampagneIds));
			
			//List to hold mapped objects
			var convertedChampagneList = new List<ChampagneLight>();

			//Map each published champagne into convertedList
			foreach (var champagne in champagnesInFolder)
			{
				if (champagne.IsPublished)
				{
					//The champagne is published! Map it into a champagneLight object
					convertedChampagneList.Add(MappingResources.MapChampagneToChampagneLight(champagne, brand.Name));
				}
			}
			
			return convertedChampagneList;
		}

		public async Task<IEnumerable<Champagne>> HandleAsync(GetChampagnes query, CancellationToken ct)
		{
			return await champagneRepository.GetPaged(query.Page, query.PageSize);
		}

		public async Task<IEnumerable<Champagne>> HandleAsync(GetBrandChampagnes query, CancellationToken ct)
		{
			return await champagneRepository.GetChampagnesByBrandId(query.BrandId); 
		}

	    public async Task<ChampagneWithRatingAndTasting> HandleAsync(GetChampagneWithRatingAndTasting query, CancellationToken ct)
	    {
		    var champagne = await champagneRepository.GetById(query.ChampagneId);
		    var ratingList = await _ratingRepository.GetAll(c => new
			    {
				    rating = c.Rating
			    },
			    c => c.Id.Equals(champagne.Id), 0, 1000000);
		    
		    var champagneWithRatingAndTasting = new ChampagneWithRatingAndTasting();

		    champagneWithRatingAndTasting.Id = champagne.Id;
		    champagneWithRatingAndTasting.BrandId = champagne.BrandId;
		    champagneWithRatingAndTasting.BottleName = champagne.BottleName;
		    champagneWithRatingAndTasting.BrandName = (await brandRepository.GetById(champagne.BrandId)).Name;
		    champagneWithRatingAndTasting.Ratings = new List<double>(ratingList.Select(x => x.rating));

		    if (query.RequesterId.Equals(Guid.Empty))
		    {
			    champagneWithRatingAndTasting.IsBookmarkedByRequester = false;
			    champagneWithRatingAndTasting.IsTastedByRequester = false;
		    }
		    else
		    {
			    if (await userRepository.ChampagneIsBookmarkedByUser(query.RequesterId, query.ChampagneId))
			    {
				    champagneWithRatingAndTasting.IsBookmarkedByRequester = true;
			    }

			    if (await userRepository.ChampagneIsTastedByUser(query.RequesterId, query.ChampagneId))
			    {
				    champagneWithRatingAndTasting.IsTastedByRequester = true;
			    }
		    }

		    var tastings = await tastingRepository.GetTastingsPagedByChampagneIdFilteredAsync(query.ChampagneId, query.Page,
			    query.PageSize, query.OrderByOption);

		    var convertedTastings = await ConvertTastings(tastings, query.RequesterId);

		    champagneWithRatingAndTasting.Tastings = convertedTastings;


		    return champagneWithRatingAndTasting;

	    }
	    
	    public async Task<IEnumerable<TastingModel>> ConvertTastings(IEnumerable<Tasting> tastings, Guid requesterId)
        {
            var convertedList = new List<TastingModel>();

            foreach (var tasting in tastings)
            {
                var tastingModel = new TastingModel();
                tastingModel.Id = tasting.Id;
                tastingModel.Review = tasting.Review;
                tastingModel.Rating = tasting.Rating;

                tastingModel.ChampagneId = tasting.ChampagneId;
                tastingModel.BrandId = tasting.BrandId;
                tastingModel.TastedOnDate = tasting.TastedOnDate;

                tastingModel.NumberOfComments = await commentRepository.CountCommentsForContextId(tasting.Id);
                tastingModel.NumberOfLikes = await likeRepository.CountLike(tasting.Id);

                var author = await userRepository.GetById(tasting.AuthorId);
	            if (author != null)
	            {
		            tastingModel.AuthorId = tasting.AuthorId;
		            tastingModel.AuthorName = author.Name;
		            tastingModel.AuthorProfileImgId = author.ImageCustomization.ProfilePictureImgId;
	            }
	            else
	            {
		            tastingModel.AuthorId = Guid.Empty;
		            tastingModel.AuthorName = "Unknown";
		            tastingModel.AuthorProfileImgId = Guid.Empty;
	            }

	            if (requesterId.Equals(Guid.Empty))
                {
                    tastingModel.IsLikedByRequester = false;
                    tastingModel.IsCommentedByRequester = false;
                }
                else
                {
                    var likeEntity = await likeRepository.GetLikeByKey(new Like.PrimaryKey
                    {
                        LikeById = requesterId,
                        LikeToContextId = tasting.Id
                    });
                    if (likeEntity != null)
                    {
                        tastingModel.IsLikedByRequester = true;
                    }

                    if (await commentRepository.IsCommentedByUser(tasting.Id, requesterId))
                    {
                        tastingModel.IsCommentedByRequester = true;
                    }
                    else
                    {
                        tastingModel.IsCommentedByRequester = false;
                    }
                    
                }
                convertedList.Add(tastingModel);
            }
            
            return convertedList;
        }

	    public async Task<IEnumerable<ChampagneLight>> HandleAsync(GetChampagnesByFilterPagedAsync query, CancellationToken ct)
	    {
		    var result = await champagneRepository
			    .GetChampagnesByFilterPagedAsync(
				    query.FilterSearchQuery.IsVintage.Vintage,
				    query.FilterSearchQuery.IsVintage.NonVintage,
				    query.FilterSearchQuery.ChampagneStyle,
				    query.FilterSearchQuery.ChampagneDosage,
				    query.FilterSearchQuery.LowerRating,
				    query.FilterSearchQuery.UpperRating,
				    query.Page,
				    query.PageSize);

		    var convertedList = new List<ChampagneLight>();
		    var brandCache = new Dictionary<Guid, Brand>();
            
		    //Map into ChampagneLight
		    foreach (var champagne in result)
		    {
			    var champagneLight = new ChampagneLight();

			    champagneLight.Id = champagne.Id;
			    champagneLight.BottleName = champagne.BottleName;
                    
			    if (brandCache.ContainsKey(champagne.BrandId))
			    {
				    var brand = brandCache[champagne.BrandId];

				    champagneLight.BrandId = champagne.BrandId;
				    champagneLight.BrandName = brand.Name;
			    }
			    else
			    {
				    var brand = await brandRepository.GetById(champagne.BrandId);
				    brandCache.Add(brand.Id, brand);

				    champagneLight.BrandId = brand.Id;
				    champagneLight.BrandName = brand.Name;
			    }

			    champagneLight.BottleImgId = champagne.BottleImgId;
			    champagneLight.IsPublished = champagne.IsPublished;
			    champagneLight.ChampagneRootId = champagne.Id; //TODO: ChampagneRootId;
			    champagneLight.NumberOfTastings = champagne.RateCount;
			    champagneLight.RatingSumOfTastings = champagne.RateValue;// MappingResources.CalculateRatingSum(champagne.RatingDictionary);
			    champagneLight.GetVintageInfo = new ChampagneLight.VintageInfo
			    {
				    IsVintage = champagne.vintageInfo.IsVintage,
				    Year = champagne.vintageInfo.Year
			    };
                
			    convertedList.Add(champagneLight);
		    }

		    return convertedList;
	    }

	    public async Task<IEnumerable<ChampagneSearchModel>> HandleAsync(SearchChampagnes query, CancellationToken ct)
	    {
		    const string CachedBrands = "GetAllBrandsCached";
		    
		    //Retrieve all brands, there is only 50 so it will be okay for now. It is okay to cache since there is a maximum of 50 brands atm, which means a dic with 50 entries... No big deal. Brand name rarely change and the same can be said about the ids :D
		    var brands = await _memoryCache.GetOrCreateAsync(CachedBrands, async entry =>
		    {
			    entry.SetSlidingExpiration(TimeSpan.FromDays(5));

			    return await brandRepository.GetAll(c => new BrandSearchProjectionModel
			    {
				    Id = c.Id,
				    ImageId = c.LogoImgId,
				    Name = c.Name
			    });
		    });
		    
		    //Regex on the brands dic and retrieve a list of ids so we can search all champagnes which also have a match on the brand name.
		    var brandIds = RegexSortList(brands, query.SearchText);
		    
		    var result = await champagneRepository.SearchChampagnes(query.SearchText, query.Page, query.PageSize, brandIds);

		    var convertedList = new List<ChampagneSearchModel>();

		    foreach (var champagne in result)
		    {
			    //Find brandName
			    string brandName = null;
			    if (brands.Any(c => c.Id.Equals(champagne.BrandId)))
			    {
				    brandName = brands.SingleOrDefault(c => c.Id.Equals(champagne.BrandId)).Name;
			    }
			    convertedList.Add(MappingResources.MapChampagneSearchProjectionModelToChampagneSearchModel(champagne, brandName));
		    }

		    return convertedList;
	    }


	    private HashSet<Guid> RegexSortList(IEnumerable<BrandSearchProjectionModel> brands, string searchText)
	    {
		    var set = new HashSet<Guid>();
		    
		    foreach (var key in brands)
		    {
			    if (MappingResources.RemoveDiacritics(key.Name.ToLower()).Contains(searchText.ToLower()))
			    {
				    set.Add(key.Id);
			    }
		    }
		    
		    return set;
	    }
    }
}