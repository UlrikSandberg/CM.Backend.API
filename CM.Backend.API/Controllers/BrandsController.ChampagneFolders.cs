using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.RequestModels;
using CM.Backend.Documents.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CM.Backend.Commands.Commands;
using CM.Backend.Domain.Aggregates.ChampagneRoot;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using Microsoft.AspNetCore.Authorization;

namespace CM.Backend.API.Controllers
{
    public partial class BrandsController
    {
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("{brandId}/champagneFolders")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> CreateChampagneFolder(Guid brandId, ChampagneFolderContentType contentType, ChampagneFolderType folderType, [Microsoft.AspNetCore.Mvc.FromBody]CreateChampagneFolderRequestModel requestModel, bool isOnDiscover = false)
        {
            var folderContent = contentType.ToString();
            if (folderContent.Equals("NonVintage"))
            {
                folderContent = "Non-Vintage";
            }
            
            var result = await CommandRouter.RouteAsync<CreateChampagneFolder, IdResponse>(new CreateChampagneFolder(
                requestModel.FolderName,
                brandId,
                requestModel.DisplayImageId,
                folderContent,
                folderType.ToString(),
                isOnDiscover));

            if (!result.IsSuccessful)
            {
                Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
                return StatusCode(400, result.Message);
            }

            return new ObjectResult(result.Id);
        }
        
        [Microsoft.AspNetCore.Mvc.HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("{brandId}/champagneFolders/{champagneFolderId}")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> EditChampagneFolder(Guid brandId, Guid champagneFolderId, ChampagneFolderContentType folderContentType, string folderName, Guid displayImageId, bool isOnDiscover = false)
        {
            var folderContent = folderContentType.ToString();
            if (folderContent.Equals("NonVintage"))
            {
                folderContent = "Non-Vintage";
            }
            
            var result = await CommandRouter.RouteAsync<EditChampagneFolder, Response>(new EditChampagneFolder(
                brandId,
                champagneFolderId,
                folderName,
                displayImageId,
                folderContent,
                isOnDiscover));

            if(!result.IsSuccessful)
            {
                Logger.Error(result.Exception, "Command returned an error. Returning HTTP error");
                return StatusCode(400, result.Message);
            }

            return new ObjectResult(result);
        }
        
        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("{brandId}/champagneFolders/{champagneFolderId}")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> DeleteChampagneFolder(Guid brandId, Guid champagneFolderId)
        {
			var response = await CommandRouter.RouteAsync<DeleteChampagneFolder, Response>(new DeleteChampagneFolder(brandId, champagneFolderId));

            if(!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }
            
            return StatusCode(201, response.IsSuccessful);
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("{brandId}/champagneFolders/{champagneFolderId}/champagnes/{champagneId}")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> AddChampagneToFolder(Guid brandId, Guid champagneFolderId, Guid champagneId)
        {
            var response =
                await CommandRouter.RouteAsync<AddChampagneToFolder, Response>(
                    new AddChampagneToFolder(brandId, champagneFolderId, champagneId));
            
            if(!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }
            
            return StatusCode(201, response.IsSuccessful);
        }
        
        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("{brandId}/champagneFolders/{champagneFolderId}/champagnes/{champagneId}")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> RemoveChampagneFromFolder(Guid brandId, Guid champagneFolderId, Guid champagneId)
        {
            var response =
                await CommandRouter.RouteAsync<RemoveChampagneFromFolder, Response>(
                    new RemoveChampagneFromFolder(brandId, champagneFolderId, champagneId));
            
            if(!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }
            
            return StatusCode(201, response.IsSuccessful);
        }
        
        //**********************************
        // GET SECTIONS
        //**********************************
        
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("{brandId}/champagneFolders")]
        public async Task<IActionResult> GetChampagneBrandRoots(Guid brandId, bool includeEmptyRoots = false)
        {
            var result = await QueryRouter.QueryAsync<GetBrandChampagneFolders, IEnumerable<ChampagneFolderQueryModel>>(new GetBrandChampagneFolders(brandId, includeEmptyRoots));

            if(result == null)
            {
                return NotFound();
            }

            return new ObjectResult(result);
        }
    }
}