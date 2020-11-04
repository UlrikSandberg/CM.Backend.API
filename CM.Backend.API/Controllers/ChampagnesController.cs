using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.EnumOptions.ChampagneFolderEnums;
using CM.Backend.API.Helpers;
using CM.Backend.API.RequestModels.ChampagneRequestModels;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Builders;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.ChampagneModels;
using CM.Backend.Queries.Model.TastingModels;
using CM.Backend.Queries.Model.UserListModels;
using CM.Backend.Queries.Queries;
using CM.Backend.Queries.Queries.TastingQueries;
using CM.Backend.Queries.Queries.UserListQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Context;
using Swashbuckle.AspNetCore.Annotations;

namespace CM.Backend.API.Controllers
{
	
	[Route("api/v1/champagnes")]
	[ServiceFilter(typeof(UnifiedEndpointNameFilter))]
	[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	public class ChampagnesController : ControllerBase
	{
		public ChampagnesController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
		{
		}

		[HttpGet]
		[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"page", "pageSize", "folderType"})]
        [Route("champagneFolders")]
        public async Task<IActionResult> GetChampagneFoldersPagedAsync(int page, int pageSize, FolderType folderType)
        {
			var result = await QueryRouter.QueryAsync<GetAllChampagneFolders, IEnumerable<ChampagneFolderQueryModel>>(new GetAllChampagneFolders(page, pageSize, folderType.ToString()));

            if (result == null)
            {
                return NotFound();
            }

            return new ObjectResult(result);
        }

		
		/// <summary>
		/// This shouldn't be a put method since it doesn't update. Though the app couldn't post the required json content i get method. Thus a put method is cheat
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="requestModel"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("filterPagedAsync")]
		public async Task<IActionResult> GetChampagneByFilterPagedAsync(int page, int pageSize,
			[FromBody]FilterSearchQueryRequestModel requestModel)
		{
			
			//Create FilterSearchQuery from requestModel
			var builder = new FilterSearchQuery.FilterSearchQueryBuilder();

			if (requestModel.IsVintage != null)
			{
				builder.SetIsVintage(requestModel.IsVintage.Vintage).SetIsNonVintage(requestModel.IsVintage.NonVintage);
			}

			builder.SetLowerRating(requestModel.LowerRating).SetUpperRating(requestModel.UpperRating);

			if (requestModel.ChampagneStyle != null)
			{
				var convertedStyleEnums = new List<PersistanceChampagneStyleEnum.ChampagneStyleEnum>();
				foreach (var style in requestModel.ChampagneStyle)
				{
					var convertedStyle = ChampagneStyleEnum.ConvertStringToStyleEnum(style);
					if (!convertedStyle.Equals(ChampagneStyleEnum.Style.Unknown))
					{
						convertedStyleEnums.Add(ChampagneStyleEnum.ConvertToPersistenceEnum(convertedStyle));
					}
				}

				builder.SetChampagneStyle(convertedStyleEnums);
			}

			if (requestModel.ChampagneDosage != null)
			{
				var convertedDosageEnums = new List<PersistenceChampagneDosageEnum.ChampagneDosageEnum>();
				foreach (var dosage in requestModel.ChampagneDosage)
				{
					var convertedDosage = DosageEnum.ConvertStringToDosageEnum(dosage);
					if (!convertedDosage.Equals(DosageEnum.Dosage.Unknown))
					{
						convertedDosageEnums.Add(DosageEnum.ConvertToPersistenceDosageEnum(convertedDosage));
					}
				}

				builder.SetChampagneDosage(convertedDosageEnums);
			}

			var result =
				await QueryRouter.QueryAsync<GetChampagnesByFilterPagedAsync, IEnumerable<ChampagneLight>>(
					new GetChampagnesByFilterPagedAsync(builder.Build(), page, pageSize));


			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		[HttpGet]
		//[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"isShuffled", "amount"})]
		[Route("champagneFolders/shuffled")]
        [SwaggerResponse(200, "Returns the roots", typeof(IEnumerable<ChampagneFolderQueryModel>))]
		public async Task<IActionResult> GetChampagneFoldersShuffled(bool isShuffled, int amount = 100)
		{
			var result =
				await QueryRouter.QueryAsync<GetChampagneFoldersShuffled, IEnumerable<ChampagneFolderQueryModel>>(
					new GetChampagneFoldersShuffled(isShuffled, amount));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}
        
		[HttpGet]
		[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"champagneFolderId"})]
		[Route("champagneFolders/{champagneFolderId}/champagnes")]
		public async Task<IActionResult> GetChampagnesFromList(Guid champagneFolderId)
		{
			var result = await QueryRouter.QueryAsync<GetChampagnesInFolder, IEnumerable<ChampagneLight>>(new GetChampagnesInFolder(champagneFolderId));
		
			if(result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);
		}

		[HttpGet]
		[Route("{champagneId}")]
		public async Task<IActionResult> GetChampagne(Guid champagneId)
		{
			var result = await QueryRouter.QueryAsync<GetChampagne, ChampagneQueryModel>(new GetChampagne(champagneId, RequestingUserId));

			if(result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		[HttpGet]
		[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"page", "pageSize"})]
		[Route("")]
		public async Task<IActionResult> GetChampagnes(int page = 100, int pageSize = 100)
		{

			var result = await QueryRouter.QueryAsync<GetChampagnes, IEnumerable<Champagne>>(new GetChampagnes(page, pageSize));

			if(result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		[HttpGet]
		//[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"champagneId", "page", "pageSize", "orderAcendingByDate"})]
		[Route("{champagneId}/tastings")]
		public async Task<IActionResult> GetChampagneTastings(Guid champagneId, int page = 0, int pageSize = 100, bool orderAcendingByDate = false)
		{
			var result =
				await QueryRouter.QueryAsync<GetChampagneTastings, IEnumerable<TastingModel>>(
					new GetChampagneTastings(champagneId, RequestingUserId, page, pageSize, orderAcendingByDate));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);

		}

		[HttpGet]
		[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"champagneId", "page", "pageSize", "orderBy"})]
		[Route("{champagneId}/ratings")]
		public async Task<IActionResult> GetChampagneRatingsAndTastings(Guid champagneId, int page, int pageSize, TastingOrderByOptions.OrderByOptions orderBy)
		{
			var result =
				await QueryRouter.QueryAsync<GetChampagneWithRatingAndTasting, ChampagneWithRatingAndTasting>(
					new GetChampagneWithRatingAndTasting(
						RequestingUserId, 
						champagneId, 
						page, 
						pageSize, 
						TastingOrderByOptions.TastingOrderByPersistenceConversion(orderBy)));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		[HttpGet]
		[ResponseCache(Duration = 0, VaryByQueryKeys = new []{"searchText", "page", "pageSize"})]
		[Route("search")]
		public async Task<IActionResult> SearchChampagne(string searchText, int page, int pageSize)
		{
			var result =
				await QueryRouter.QueryAsync<SearchChampagnes, IEnumerable<ChampagneSearchModel>>(
					new SearchChampagnes(searchText, page, pageSize));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);
		}

		[HttpGet]
		[ResponseCache(Duration = 60 * 60)]
		[Route("Top10StandardLists")]
		public async Task<IActionResult> GetTop10StandardList()
		{
			var result =
				await QueryRouter.QueryAsync<GetStandardTop10Lists, IEnumerable<StandardUserList>>(
					new GetStandardTop10Lists());

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		[HttpGet]
		[Route("Top10StandardList")]
		public async Task<IActionResult> GetTop10StandardList(string configurationKey, bool filterByVintage,
			bool filterByHighestRating)
		{
			var result = await QueryRouter.QueryAsync<GetStandardTop10List, IEnumerable<ChampagneLight>>(
				new GetStandardTop10List(configurationKey, filterByVintage, filterByHighestRating));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}
	}
}