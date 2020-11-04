using System;
using System.Collections.Generic;
using CM.Backend.API.RequestModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CM.Backend.API.EnumOptions;
using CM.Backend.Commands.Commands;
using CM.Backend.Documents.Responses;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CM.Backend.API.Controllers
{
	public partial class BrandsController
	{
		[HttpPost]
		[Route("{brandId}/champagnes")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> CreateChampagne(Guid brandId, [FromBody]CreateChampagneRequest request, CreateChampagneWithOptions createChampagneWithOptions, Guid champagneFolderId, bool isOnDiscover = false)
		{
			var result = await CommandRouter.RouteAsync<CreateChampagne, IdResponse>(
				new CreateChampagne(request.BottleName, brandId, request.BottleImgId, request.Vintage, request.Year, createChampagneWithOptions.ToString(), champagneFolderId, isOnDiscover));
			
			if (!result.IsSuccessful)
			{
				Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, result.Message);
			}

			return new ObjectResult(result.Id);
		}

		/// <summary>
		/// Edits the champagne. Not supported yet
		/// </summary>
		/// <returns>The champagne.</returns>
		/// <param name="brandId">Brand identifier.</param>
		/// <param name="champagneId">Champagne identifier.</param>
		[HttpPut]
		[Route("{brandId}/champagnes/{champagneId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> EditChampagne(Guid brandId, Guid champagneId, [FromBody]EditChampagneRequest editChampagneRequest)
		{

			var result = await CommandRouter.RouteAsync<EditChampagne, Response>(new EditChampagne(editChampagneRequest.BottleName, brandId, champagneId, editChampagneRequest.BottleImgId, editChampagneRequest.IsVintage, editChampagneRequest.VintageYear));

			if(!result.IsSuccessful)
			{
				Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, result.Message);
			}


			return StatusCode(200);
		}

		[HttpGet]
		[Route("{brandId}/champagnes")]
		public async Task<IActionResult> GetChampagnes(Guid brandId)
		{

			var result = await QueryRouter.QueryAsync<GetBrandChampagnes, IEnumerable<Champagne>>(new GetBrandChampagnes(brandId));

			if(result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		[HttpPost]
		[Route("{brandId}/champagnes/{champagneId}/profile")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> AddChampagneProfile(Guid champagneId, Guid brandId, [FromBody]AddChampagneProfileRequest profileRequest)
		{
			var result = await CommandRouter.RouteAsync<AddChampagneProfile, Response>(new AddChampagneProfile(
				brandId,
				champagneId,
				profileRequest.Appearance,
				profileRequest.BlendInfo,
				profileRequest.Taste,
				profileRequest.FoodPairing,
				profileRequest.Aroma,
				profileRequest.BottleHistory,
				profileRequest.DisplayDosage,
				profileRequest.DosageCodes,
				profileRequest.DisplayStyle,
				profileRequest.StyleCodes,
				profileRequest.DisplayCharacter,
				profileRequest.CharacterCodes,
				profileRequest.RedWineAmount,
				profileRequest.ServingTemp,
				profileRequest.AgeingPotential,
				profileRequest.ReserveWineAmount,
				profileRequest.DosageAmount,
				profileRequest.AlchoholVol,
				profileRequest.PinotNoir,
				profileRequest.PinotMeunier,
				profileRequest.Chardonnay));

			if (!result.IsSuccessful)
			{
				Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, result.Message);

			}

			return StatusCode(200);
		}

		/// <summary>
		/// Edits the champagne profile.
		/// </summary>
		/// <returns>The champagne profile.</returns>
		/// <param name="brandId">Brand identifier.</param>
		/// <param name="champagneId">Champagne identifier.</param>
		[HttpPut]
		[Route("{brandId}/champagnes/{champagneId}/profile")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> EditChampagneProfile(Guid brandId, Guid champagneId, [FromBody]EditChampagneProfileRequestModel editCPRM)
		{
			var response = await CommandRouter.RouteAsync<EditChampagneProfile, Response>(new EditChampagneProfile(
				brandId,
				champagneId,
                editCPRM.Appearance,
				editCPRM.BlendInfo,
				editCPRM.Taste,
				editCPRM.FoodPairing,
				editCPRM.Aroma,
				editCPRM.BottleHistory,
				editCPRM.DisplayDosage,
				editCPRM.DosageCodes,
				editCPRM.DisplayStyle,
				editCPRM.StyleCodes,
				editCPRM.DisplayCharacter,
				editCPRM.CharacterCodes,
				editCPRM.RedWineAmount,
				editCPRM.ServingTemp,
				editCPRM.AgeingPotential,
				editCPRM.ReserveWineAmount,
				editCPRM.DosageAmount,
				editCPRM.AlchoholVol,
				editCPRM.PinotNoir,
				editCPRM.PinotMeunier,
				editCPRM.Chardonnay 
			));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			return StatusCode(201);
		}

		[HttpGet]
		[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId", "champagneFolderId", "amount"})]
		[Route("{brandId}/champagneFolders/{champagneFolderId}")]
		public async Task<IActionResult> GetChampagneFolderAsync(Guid brandId, Guid champagneFolderId, int amount = 100)
		{
			var result = await QueryRouter.QueryAsync<GetChampagneFolder, ChampagneFolderQueryModel>(new GetChampagneFolder(champagneFolderId));

			if(result == null)
			{
				return NotFound();
			}
         
			return new ObjectResult(result);
		}
		

		[HttpGet]
		[ResponseCache(Duration = 60, VaryByQueryKeys = new []{"brandId", "isShuffled", "amount"})]
		[Route("{brandId}/champagneFolders/shuffled")]
		public async Task<IActionResult> GetBrandFoldersShuffled(Guid brandId, bool isShuffled = true, int amount = 100)
		{
			
			var result = await QueryRouter.QueryAsync<GetBrandChampagneFoldersShuffled, IEnumerable<ChampagneFolderQueryModel>>(new GetBrandChampagneFoldersShuffled(brandId, isShuffled, amount));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
			
		}

        [HttpPut]
		[Route("{brandId}/champagnes/{champagneId}/publishStatus")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
		public async Task<IActionResult> SetChampagnePublishStatus(Guid brandId, Guid champagneId, bool IsPublished)
		{

			var response = await CommandRouter.RouteAsync<SetChampagnePublishingStatus, Response>(new SetChampagnePublishingStatus(brandId, champagneId, IsPublished));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, "Something went horribly wrong");
			}

			return StatusCode(201, response.IsSuccessful);

		}
	}
}
