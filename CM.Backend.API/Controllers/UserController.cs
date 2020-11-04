using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using CM.Backend.API.Helpers;
using CM.Backend.API.RequestModels.UserRequestModels;
using CM.Backend.Commands.Commands.UserCommands;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net;
using System.Net.Http;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using System.Net.Http.Headers;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.Commands.Commands.BugAndFeedbackCommands;
using CM.Backend.Commands.Commands.NotificationCommands;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model;
using SixLabors.ImageSharp;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using CM.Backend.Queries.Queries.UserQueries;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.TastingModels;
using CM.Backend.Queries.Model.UserModels;
using CM.Backend.Queries.Queries.TastingQueries;
using CM.Backend.Queries.Queries.UserCreationQuerires;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;

namespace CM.Backend.API.Controllers
{
	
	[Route("api/v1/users")]
	[ServiceFilter(typeof(UnifiedEndpointNameFilter))]
	public partial class UserController : ControllerBase
	{
		private readonly IOptions<ProjectionsPersistenceConfiguration> _projectionsConfig;
		private readonly IOptions<IdentityServerConfiguration> _identityConfig;

		public UserController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<ProjectionsPersistenceConfiguration> projectionsConfig, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
		{
			_projectionsConfig = projectionsConfig;
			_identityConfig = identityConfig;
		}
        
        /// <summary>
        /// Gets a user by id. Do not use to retreive currentUser. To get current user use /api/v1/users/currentUser.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		[HttpGet]
		[Route("{userId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUser(Guid userId)
		{
			var result = await QueryRouter.QueryAsync<GetPublicUser, PublicUserQueryModel>(new GetPublicUser(userId, RequestingUserId));
			
            if(result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);
		}

		/// <summary>
		/// Search users pagedAsync by their username. The search uses a case insensitive regex search.
		/// A new search has been added! /search --> This search both usernames and profilename in the same
		/// regex search... Use instead if both results is wanted...
		/// </summary>
		/// <param name="username"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("userSearch/username")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> SearchUserByName(string username, int page, int pageSize)
		{
			var result =
				await QueryRouter.QueryAsync<SearchUsersByUsername, IEnumerable<FollowingQueryModel>>(
					new SearchUsersByUsername(RequestingUserId, username, page, pageSize));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}
		
		/// <summary>
		/// Search users pagedAsync by their profilename. The search uses a case insensitive regex search.
		/// </summary>
		/// <param name="profilename"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("userSearch/profilename")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> SearchUserByProfileName(string profilename, int page, int pageSize)
		{
			var result =
				await QueryRouter.QueryAsync<SearchUsersByProfilename, IEnumerable<FollowingQueryModel>>(
					new SearchUsersByProfilename(RequestingUserId, profilename, page, pageSize));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		/// <summary>
		/// Find users unfiltered pagedAsync. Returns followingQueryModel.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("userSearch/paged")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUsersPagedAsync(int page, int pageSize)
		{
			var result =
				await QueryRouter.QueryAsync<SearchUsersPagedAsync, IEnumerable<FollowingQueryModel>>(
					new SearchUsersPagedAsync(RequestingUserId, page, pageSize));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		/// <summary>
		/// url/userSearch/paged --> Retreives users as FollowingQueryModels. Whereas
		/// url/searchPaged --> Retreives users as UserSearchProjectionModels.
		/// This is a lighter model with less data and the query is faster opposite to url/userSearch/paged
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("searchPaged")]
		public async Task<IActionResult> GetUserSearchModelsPagedAsync(int page, int pageSize)
		{
			var result =
				await QueryRouter.QueryAsync<SearchUsersPagedAsyncLight, IEnumerable<UserSearchProjectionModel>>(
					new SearchUsersPagedAsyncLight(page, pageSize));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		/// <summary>
		/// Searches users on both username and profilename at the same time.
		/// </summary>
		/// <param name="searchText"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("search")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> SearchUsersPagedAsync(string searchText, int page, int pageSize)
		{
			var result =
				await QueryRouter
					.QueryAsync<SearchUsersByProfilenameAndUsernamePagedAsync, IEnumerable<UserSearchProjectionModel>>(
						new SearchUsersByProfilenameAndUsernamePagedAsync(searchText, page, pageSize));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}
		
		/// <summary>
		/// Retreive the respective tasting with specific data fields used in respect to editing a tasting
		/// 
		/// </summary>
		/// <param name="champagneId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("currentUser/tastings/{champagneId}")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetTastingForEdit(Guid champagneId)
		{
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}

			var result = await QueryRouter.QueryAsync<GetUserTastingForEdit, EditTastingModel>(new GetUserTastingForEdit(RequestingUserId,champagneId));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		/// <summary>
		/// Gets the currentUser. Take no id as param because it uses the subject id from the bearer token
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("currentUser")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetCurrentUser()
		{
			
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}
			
			var result = await QueryRouter.QueryAsync<GetUser, UserQueryModel>(new GetUser(RequestingUserId));

			if (result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		/// <summary>
		/// Use as a submission endpoint for bugs, suggestion and feedback.
		/// </summary>
		/// <param name="requestModel"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("currentUser/submission")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> SubmitFeedbackAndBug([FromBody] FeedbackAndBugSubmissionRequestModel requestModel)
		{
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}

			var convertedType = SubmissionTypeEnum.ConvertStringToSubmissionType(requestModel.Type);
			var imageId = Guid.Empty;
			
			
			if (requestModel.Image != null)
			{
				var imageResponse = await UploadUserImagesFromByteArray(RequestingUserId, requestModel.Image, "jpg");
				if (imageResponse.IsSuccesfull)
				{
					imageId = imageResponse.ImageId;
				}
			}
			
			var response = await CommandRouter.RouteAsync<SubmitBugOrFeedback, Response>(new SubmitBugOrFeedback(RequestingUserId, convertedType.ToString(), requestModel.Content, imageId, requestModel.MayBeContacted));

			if (!response.IsSuccessful)
			{
				return StatusCode(401, response.Message);
			}

			return StatusCode(201);

		}

		/// <summary>
		/// Get users paged for internal use, returns an IEnumerable<UserQueryLight>
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUsers(int page = 0, int pageSize = 100)
		{
			var result = await QueryRouter.QueryAsync<GetAllUsers, IEnumerable<UserQueryLight>>(new GetAllUsers(page, pageSize));
            
			if(result == null)
			{
				return NotFound();
			}

			return new ObjectResult(result);

		}

		/// <summary>
		/// Retreives followers pagedAsync for the given userId, this method likewise takes the subject id from the bearer token if such exists and indicates in the result models if the asking user(subjectId) is already following the followers
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{userId}/followers")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUserFollowers(Guid userId, int page = 0, int pageSize = 100)
		{
			
			var result = await QueryRouter.QueryAsync<GetUserFollowers, IEnumerable<FollowersQueryModel>>(new GetUserFollowers(RequestingUserId, userId, page, pageSize));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);

		}

		/// <summary>
		/// Page through the indicated userId's collection of user the userId is following or and the brand which the userId is following, respectively on toggle.
		/// If the request has a valid bearer token with non.empty subjectId the response will also indicate whether the requester(subjectId) is following the entities returned.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="getUserFollowing, toggle to switch between users and brands"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{userId}/following")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUserFollowing(Guid userId, int page = 0, int pageSize = 100, bool getUserFollowing = true)
		{
			
			object result = null;
			
			if (getUserFollowing)
			{
				result =
					await QueryRouter.QueryAsync<GetUserFollowing, IEnumerable<FollowingQueryModel>>(
						new GetUserFollowing(RequestingUserId, userId, page, pageSize));
			}
			else
			{
				result = 
					await QueryRouter.QueryAsync<GetUserBrandFollowing, IEnumerable<FollowingQueryModel>>(
						new GetUserBrandFollowing(RequestingUserId, userId, page, pageSize));
			}
			
			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);

		}
		
        /// <summary>
        /// For single upload purposes. Currently not in use by app, might come in handy if we want to upload a single image. For now see updateSettingsFromBuilder...
		/// ***** Repeat ***** Do not use this atm
        /// </summary>
        /// <returns>The profile cover.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="image">Image.</param>
		[HttpPut]
		[Route("currentUser/profileCoverImages")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateProfileCover(IFormFile image)
		{
			throw new NotImplementedException();
			/*
			var response = await UploadUserImages(userId, image);
            
			if(!response.IsSuccesfull)
			{
				return StatusCode(response.StatusCode, response.Message);
			}

			var commandResponse = await CommandRouter.RouteAsync<UpdateProfileCover, Response>(new UpdateProfileCover(userId, response.ImageId));

			if(!commandResponse.IsSuccessful)
			{
				return StatusCode(400, "Something went wrong");
			}

			return StatusCode(201, commandResponse.IsSuccessful);
			*/
		}

        /// <summary>
        /// For single upload purposes. Currently not in use by app, might come in handy if we want to upload a single image. For now see updateSettingsFromBuilder...
		/// ***** Repeat ***** Do not use this atm
        /// </summary>
        /// <returns>The profile image.</returns>
        /// <param name="userId">User identifier.</param>
        /// <param name="image">Image.</param>
		[HttpPut]
		[Route("currentUser/profileImages")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateProfileImage(IFormFile image)
		{
			throw new NotImplementedException();
			/*var response = await UploadUserImages(userId, image);

			if(!response.IsSuccesfull)
			{
				return StatusCode(response.StatusCode, response.Message);
			}

			var commandResponse = await CommandRouter.RouteAsync<UpdateProfileImage, Response>(new UpdateProfileImage(userId, response.ImageId));

			if(!commandResponse.IsSuccessful)
			{
				return StatusCode(400, "Something went wrong");
			}

			return StatusCode(201, commandResponse.IsSuccessful);*/
		}
        
		/// <summary>
		/// For single upload purposes. Currently not in use by app, might come in handy if we want to upload a single image. For now see updateSettingsFromBuilder...
		/// ***** Repeat ***** Do not use this atm
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("currentUser/cellarCardImg")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateCellarCardImage(IFormFile image)
		{
			throw new NotImplementedException();
			/*var response = await UploadUserImages(userId, image);

			if(!response.IsSuccesfull)
			{
				return StatusCode(response.StatusCode, response.Message);
			}

			var commandResponse = await CommandRouter.RouteAsync<UpdateCellarCardImage, Response>(new UpdateCellarCardImage(userId, response.ImageId));
				
			if(!commandResponse.IsSuccessful)
			{
				return StatusCode(400, "Something went wrong");
			}

			return StatusCode(201, commandResponse.IsSuccessful);*/

		}
		
		/// <summary>
		/// For single upload purposes. Currently not in use by app, might come in handy if we want to upload a single image. For now see updateSettingsFromBuilder...
		/// ***** Repeat ***** Do not use this atm
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="image"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("currentUser/cellarHeaderImg")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateCellarHeaderImg(IFormFile image)
		{
			throw new NotImplementedException();
			/*var response = await UploadUserImages(userId, image);

			if(!response.IsSuccesfull)
			{
				return StatusCode(response.StatusCode, response.Message);
			}

			var commandResponse = await CommandRouter.RouteAsync<UpdateCellarHeaderImage, Response>(new UpdateCellarHeaderImage(userId, response.ImageId));

			if(!commandResponse.IsSuccessful)
			{
				return StatusCode(400, "Not succesfull");
			}

			return StatusCode(201, commandResponse.IsSuccessful);*/

		}

        /// <summary>
        /// Updates the user settings. 
        /// </summary>
        /// <returns>The user settings.</returns>
        /// <param name="userId">User identifier.</param>
		[HttpPut]
		[Route("currentUser/settings")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateUserSettings(Guid userId, [FromBody]EditUserSettingsRequestModel editUserSettings)
        {
	        if (RequestingUserId.Equals(Guid.Empty))
	        {
		        return StatusCode(401);
	        }
			
			var response = await CommandRouter.RouteAsync<EditUserSettings, Response>(new EditUserSettings(userId, editUserSettings.CountryCode, editUserSettings.Language));

			if(!response.IsSuccessful)
			{
				return StatusCode(400, "Something went wrong");
			}

			return StatusCode(201, response.IsSuccessful);
		}

		/// <summary>
		/// Updates the users settings. Use the UpdateSettingsBuilder. Only field which are not null will be updated.
		/// </summary>
		/// <param name="requestModel"></param>
		/// <returns></returns>
		[HttpPut]
		[Route("currentUser/updateSettingsFromBuilder")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateUserSettingsFromBuilder([FromBody]UpdateUserSettingsRequestModel requestModel)
		{
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}

			var updateCommand = new UpdateUserSettings(RequestingUserId);
			
			//If the name has changed we will firstly have to check if the name havn't already been taken. 
			if (requestModel.Name != null)
			{

				var response =
					await QueryRouter.QueryAsync<CheckUsernameAvailabillity, bool>(
						new CheckUsernameAvailabillity(requestModel.Name));

				if (!response)
				{
					return StatusCode(400, "Username is already being used by somebody else");
				}
			}

			updateCommand.Name = requestModel.Name;
			updateCommand.ProfileName = requestModel.ProfileName;
			updateCommand.Biography = requestModel.Biography;
			
			//If Any pictures have changed, upload and retreive id for each.
			if (requestModel.ProfileImage != null)
			{
				var imageResponse = await UploadUserImagesFromByteArray(RequestingUserId, requestModel.ProfileImage, "jpg");
				if (imageResponse.IsSuccesfull)
				{
					updateCommand.ProfileImageId = imageResponse.ImageId;
				}
			}
			if (requestModel.ProfileCoverImage != null)
			{
				var imageResponse = await UploadUserImagesFromByteArray(RequestingUserId, requestModel.ProfileCoverImage, "jpg");
				if (imageResponse.IsSuccesfull)
				{
					updateCommand.ProfileCoverImageId = imageResponse.ImageId;
				}
			}
			if (requestModel.ProfileCellarCardImage != null)
			{
				var imageResponse = await UploadUserImagesFromByteArray(RequestingUserId, requestModel.ProfileCellarCardImage, "jpg");
				if (imageResponse.IsSuccesfull)
				{
					updateCommand.ProfileCellarCardImageId = imageResponse.ImageId;
				}
			}

			var ShouldUpdate = false;
			foreach (var property in updateCommand.GetType().GetProperties())
			{
				if (property.GetValue(updateCommand) != null)
				{
					ShouldUpdate = true;
				}
			}
			
			//Update user
			if (ShouldUpdate)
			{
				var commandResponse = await CommandRouter.RouteAsync<UpdateUserSettings, Response>(updateCommand);

				if (!commandResponse.IsSuccessful)
				{
					return StatusCode(400, commandResponse.Message);
				}
			}

			return StatusCode(201);
		}

		[HttpPut]
		[Route("currentUser/updateUserNotificationSettingsFromBuilder")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> UpdateUserNotificationSettings(
			[FromBody] UpdateUserNotificationSettings requestModel)
		{
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}

			var response = await CommandRouter.RouteAsync<UpdateUserNotificationSettings, Response>(new UpdateUserNotificationSettings(RequestingUserId, requestModel.ReceiveCMNotifications, requestModel.NotifyNewFollower, requestModel.NotifyNewComment, requestModel.NotifyActivityInThread, requestModel.NotifyLikeTasting, requestModel.NotifyLikeComment, requestModel.ReceiveNewsLetter, requestModel.NotifyChampagneTasted, requestModel.NotifyBrandNews));
			
			return !response.IsSuccessful ? NotFound() : StatusCode(201);
		}
		
		/// <summary>
		/// Page through the indicated userId's collection of tasted or and bookmarked champagnes, respectively on toggle.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="getTasted indicates whether you are paging the currentUsers tasted collection or saved. The tasted collection is given by a UserCellarChampagneQueryModel where as the
		/// saved collection is a ChampagneLightModel"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{userId}/cellar")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUserCellar(Guid userId, int page, int pageSize, bool getTasted = true)
		{

			object result = null;

			if (getTasted)
			{
				result = await QueryRouter.QueryAsync<GetUserCellarPaged, IEnumerable<UserCellarChampagneQueryModel>>(
					new GetUserCellarPaged(userId, page, pageSize));
			}
			else
			{
				result = await QueryRouter.QueryAsync<GetUserCellarSavedPaged, IEnumerable<ChampagneLight>>(new GetUserCellarSavedPaged(userId, page, pageSize));

			}

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}
		
		
		/// <summary>
		/// Page through the currentUsers collection of tasted or and bookmarked champagnes, respectively on toggle.
		/// </summary>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <param name="getTasted, indicates whether you are paging the currentUsers tasted collection or saved. The tasted collection is given by a UserCellarChampagneQueryModel where as the
		/// saved collection is ChampagneLightModel"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("currentUser/cellar")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetCurrentUserCellar(int page, int pageSize, bool getTasted = true)
		{
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}

			object result = null;
			
			if (getTasted)
			{
				result =
					await QueryRouter.QueryAsync<GetUserCellarPaged, IEnumerable<UserCellarChampagneQueryModel>>(
						new GetUserCellarPaged(RequestingUserId, page, pageSize));
			}
			else
			{
				result = await QueryRouter.QueryAsync<GetUserCellarSavedPaged, IEnumerable<ChampagneLight>>(new GetUserCellarSavedPaged(RequestingUserId, page, pageSize));

			}

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		/// <summary>
		/// Get the most essential userInfo for certain update cenarios in app
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("{userId}/userInfo")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetUserInfo(Guid userId)
		{
			var result = await QueryRouter.QueryAsync<GetUserInfo, UserInfoQueryModel>(new GetUserInfo(userId));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}

		/// <summary>
		/// Returns the currentUsers settings.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("currentUser/settings")]
		[Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
		public async Task<IActionResult> GetCurrentUserSettings()
		{
			if (RequestingUserId.Equals(Guid.Empty))
			{
				return StatusCode(401);
			}

			var result = await QueryRouter.QueryAsync<GetUserSettings, UserSettingsModel>(new GetUserSettings(RequestingUserId));

			if (result == null)
			{
				return NotFound();
			}
			
			return new ObjectResult(result);
		}
		
		private async Task<ImageUploadResponse> UploadUserImages(Guid userId, IFormFile image)
		{
			string fileExtension = Path.GetExtension(image.FileName).TrimStart('.');

            if (!fileExtension.Equals("jpg") && !fileExtension.Equals("png"))
            {
				return new ImageUploadResponse(false, Guid.Empty, "Incompatible file format. File must be of type .jpg or .png", 415);
            }

			var isSuccessful = false;
			var storageConnectionString = _projectionsConfig.Value.AzureStorageAccountConnectionString;
            var fileId = Guid.Empty;

			//Establish connection to the blob storage
			if (CloudStorageAccount.TryParse(storageConnectionString, out var cloudStorageAccount))
			{
				try
				{
					//Init blobClient
					var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
					var cloudBlobContainer = cloudBlobClient.GetContainerReference("user-files");

					//Connection established, read file stream and upload file to cloudBlobContainer.
					using (var ms = new MemoryStream())
					{
						await image.CopyToAsync(ms);

						fileId = Guid.NewGuid();
						var file = ms.ToArray();

						//Convert png images to jpg since they are much smaller
                        
						if (fileExtension.Equals("png"))
						{
							using (var conversionMS = new MemoryStream())
							{
								var jpg = Image.Load(file);
								jpg.SaveAsJpeg(conversionMS);
								file = conversionMS.ToArray();
								fileExtension = "jpg";
							}
						}
                        
						var fileName = "original." + fileExtension;

						var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + fileId.ToString() + "/" + fileName);
						try
						{
							await cloudBlockBlob.UploadFromByteArrayAsync(file, 0, file.Length);
							isSuccessful = true;//TODO : 
						}
						catch (StorageException ex)
						{
							Console.WriteLine(ex.StackTrace);
							isSuccessful = false;
						}
					}
				}
				catch (StorageException ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
			
	        return isSuccessful ? new ImageUploadResponse(true, fileId, null, 200) : new ImageUploadResponse(false, Guid.Empty, "Failed upload to blobstorage try again", 400);          
		}
		
		private async Task<ImageUploadResponse> UploadUserImagesFromByteArray(Guid userId, byte[] image, string fileExtension)
		{
			var isSuccessful = false;
			var storageConnectionString = _projectionsConfig.Value.AzureStorageAccountConnectionString;
            var fileId = Guid.Empty;

			//Establish connection to the blob storage
			if (CloudStorageAccount.TryParse(storageConnectionString, out var cloudStorageAccount))
			{
				var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
				try
				{
					//Init blobClient
					var cloudBlobContainer = cloudBlobClient.GetContainerReference("user-files");

					fileId = Guid.NewGuid();
					var file = image;

					var fileName = "original." + fileExtension;

					var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(userId.ToString() + "/" + "images" + "/" + fileId.ToString() + "/" + fileName);
					try
					{
						await cloudBlockBlob.UploadFromByteArrayAsync(file, 0, file.Length);
						isSuccessful = true;
					}
					catch (StorageException ex)
					{
						Console.WriteLine(ex.StackTrace);
						Logger.Fatal(ex, "Error catched in UserController --> CM.Backend.API.Controllers.UserController --> Task<IActionResult> UploadImageFromByteArray cs:line 785");
						isSuccessful = false;
					}
					
				}
				catch (StorageException ex)
				{
					Logger.Fatal(ex, "Error catched in UserController --> CM.Backend.API.Controllers.UserController --> Task<IActionResult> UploadImageFromByteArray cs:line 785");
					Console.WriteLine(ex.Message);
				}
			}
			
	        if (isSuccessful)
            {
                return new ImageUploadResponse(true, fileId, null, 200);
            }
            else
            {
                return new ImageUploadResponse(false, Guid.Empty, "Failed upload to blobstorage try again", 400);
            }          
		}
	}
}
