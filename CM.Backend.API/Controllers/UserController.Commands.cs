using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.RequestModels.TastingRequestModels;
using CM.Backend.API.RequestModels.UserRequestModels;
using CM.Backend.Commands.Commands.TastingCommands;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using Marten.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CM.Backend.API.Controllers
{
	
    public partial class UserController
    {
	    
        /// <summary>
        /// Follows the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userId">User identifier. The user invoking this action</param>
        /// <param name="followToId">Follow to user identifier. The user we are about to follow</param>
		[HttpPost]
		[Route("currentUser/following")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> FollowUser([FromBody]FollowUserRequestModel followUserRequest)
		{
			if (RequestingUserId == Guid.Empty)
			{
				return StatusCode(401);
			}

			var response = await CommandRouter.RouteAsync<FollowUser, Response>(new FollowUser(RequestingUserId, followUserRequest.FollowToUserId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			return StatusCode(200, response.IsSuccessful);
		}

        /// <summary>
        /// Unfollows the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="userId">User identifier. The user invoking this method</param>
        /// <param name="followToId">Follow to user identifier. The user we are about to unfollow</param>
		[HttpDelete]
		[Route("currentUser/following/{followToId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UnfollowUser(Guid followToId)
		{
			if (RequestingUserId == Guid.Empty)
			{
				return StatusCode(401);
			}
			
			var response = await CommandRouter.RouteAsync<UnfollowUser, Response>(new UnfollowUser(RequestingUserId, followToId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			//Unfollow another user
			return StatusCode(200, response.IsSuccessful);
		}
        
        
        /// <summary>
        /// Follows the brand.
        /// </summary>
        /// <returns>The brand.</returns>
        /// <param name="userId">User identifier. The user invoking this method</param>
        /// <param name="brandId">Brand identifier. The brand we are about to follow</param>
		[HttpPost]
		[Route("currentUser/followingBrands")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> FollowBrand([FromBody]FollowBrandRequestModel followBrandRequest)
        {
	        if (RequestingUserId == Guid.Empty)
	        {
		        return StatusCode(401);
	        }
			
			var response = await CommandRouter.RouteAsync<FollowBrand, Response>(new FollowBrand(RequestingUserId, followBrandRequest.BrandId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			//Follow a brand
			return StatusCode(200, response.IsSuccessful);
		}

        /// <summary>
        /// Unfollows the brand.
        /// </summary>
        /// <returns>The brand.</returns>
        /// <param name="userId">User identifier. The user invoking this method</param>
        /// <param name="brandId">Brand identifier. The brand we are about to unfollow</param>
		[HttpDelete]
		[Route("currentUser/followingBrands/{brandId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UnfollowBrand(Guid brandId)
        {
	        if (RequestingUserId == Guid.Empty)
	        {
		        return StatusCode(401);
	        }
			
			var response = await CommandRouter.RouteAsync<UnfollowBrand, Response>(new UnfollowBrand(RequestingUserId, brandId));
            
			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			//Unfollow brand
			return StatusCode(200, response.IsSuccessful);
		}
        
        
        /// <summary>
        /// Bookmarks the champagne.
        /// </summary>
        /// <returns>The champagne.</returns>
        /// <param name="userId">User identifier. The user invoking this method</param>
        /// <param name="champagneId">Champagne identifier. The champagne we are about to bookmark</param>
		[HttpPost]
		[Route("currentUser/bookmarkedChampagnes")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> BookmarkChampagne([FromBody]BookmarkChampagneRequestModel bookmarkChampagne)
        {
	        if (RequestingUserId == Guid.Empty)
	        {
		        return StatusCode(401);
	        }
	        
			var response = await CommandRouter.RouteAsync<BookmarkChampagne, Response>(new BookmarkChampagne(RequestingUserId, bookmarkChampagne.champagneId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			return StatusCode(200, response.IsSuccessful);
		}

        /// <summary>
        /// Unbookmarks the champagne.
        /// </summary>
        /// <returns>The champagne.</returns>
        /// <param name="userId">User identifier. The user invoking this method</param>
        /// <param name="champagneId">Champagne identifier. The champagne we are about to unbookmark</param>
		[HttpDelete]
		[Route("currentUser/bookmarkedChampagnes")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UnbookmarkChampagne(Guid champagneId)
        {
	        if (RequestingUserId == Guid.Empty)
	        {
		        return StatusCode(401);
	        }
			
			var response = await CommandRouter.RouteAsync<UnbookmarkChampagne, Response>(new UnbookmarkChampagne(RequestingUserId, champagneId));

			if(!response.IsSuccessful)
			{
				Logger.Error(response.Exception, "Command returned an error. Returning HTTP error");
				return StatusCode(400, response.Message);
			}

			//Unbookmark a champagne
			return StatusCode(200, response.IsSuccessful);
		}

	    /// <summary>
	    /// Likes an entity given by it's id. Since this could be multiple different entities, this is named contextId
	    /// </summary>
	    /// <param name="contextId"></param>
	    /// <param name="contextType"></param>
	    /// <returns></returns>
	    [HttpPost]
	    [Route("currentUser/likedEntities/{contextId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	    public async Task<IActionResult> LikeEntity(Guid contextId, LikeContextTypes.contextTypes contextType)
	    {
		    if (RequestingUserId == Guid.Empty)
		    {
			    return StatusCode(401);
		    }

		    var response = await CommandRouter.RouteAsync<LikeEntity, Response>(new LikeEntity(RequestingUserId, contextId, contextType.ToString()));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }

		    return StatusCode(200, response.IsSuccessful);

	    }

	    /// <summary>
	    /// Unlike an entity given by it's id. Since this could be multiple different entities this is given by contextId
	    /// </summary>
	    /// <param name="contextId"></param>
	    /// <returns></returns>
	    [HttpDelete]
	    [Route("currentUser/likedEntities/{contextId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	    public async Task<IActionResult> UnlikeEntity(Guid contextId)
	    {
		    if (RequestingUserId == Guid.Empty)
		    {
			    return StatusCode(401);
		    }

		    var response = await CommandRouter.RouteAsync<UnlikeEntity, Response>(new UnlikeEntity(RequestingUserId, contextId));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }

		    return StatusCode(200, response.IsSuccessful);
	    }
	    

	    /// <summary>
	    /// Create a tasting for the champagneId given. Uses userId from bearer token
	    /// </summary>
	    /// <param name="champagneId"></param>
	    /// <returns></returns>
	    [HttpPost]
	    [Route("currentUser/tastings/{champagneId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	    public async Task<IActionResult> CreateTasting(Guid champagneId, [FromBody]CreateTastingRequestModel createTastingRequestModel)
	    {
		    if (RequestingUserId == Guid.Empty)
		    {
			    return StatusCode(401);
		    }

		    var response = await CommandRouter.RouteAsync<CreateTasting, IdResponse>(new CreateTasting(RequestingUserId, champagneId,
			    createTastingRequestModel.Review, createTastingRequestModel.Rating, createTastingRequestModel.TimeStamp));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }
		    
		    return new ObjectResult(response.Id);
	    }

	    /// <summary>
	    /// Edit a tasting respective to the tastingId given. Uses userId from bearer token
	    /// </summary>
	    /// <param name="champagneId"></param>
	    /// <returns></returns>
	    [HttpPut]
	    [Route("currentUser/tastings/{tastingId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	    public async Task<IActionResult> UpdateTasting(Guid tastingId, [FromBody]EditTastingRequestModel requestModel)
	    {
		    if (RequestingUserId.Equals(Guid.Empty))
		    {
			    return StatusCode(401);
		    }

		    var response = await CommandRouter.RouteAsync<EditTasting, Response>(new EditTasting(tastingId, RequestingUserId, requestModel.Review, requestModel.Rating));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }
		    
		    return StatusCode(201);
		      
	    }

	    /// <summary>
	    /// Deletes a tasting respective to the tastingId given. Uses userId from bearer token
	    /// </summary>
	    /// <param name="tastingId"></param>
	    /// <param name="champagneId"></param>
	    /// <returns></returns>
	    [HttpDelete]
	    [Route("currentUser/tastings/{tastingId}")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	    public async Task<IActionResult> DeleteTasting(Guid tastingId)
	    {
		    if (RequestingUserId.Equals(Guid.Empty))
		    {
			    return StatusCode(401);
		    }

		    var response =
			    await CommandRouter.RouteAsync<DeleteTasting, Response>(new DeleteTasting(tastingId, RequestingUserId));

		    if (!response.IsSuccessful)
		    {
			    return StatusCode(400, response.Message);
		    }

		    return StatusCode(201);
	    }
    }
}
