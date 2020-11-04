using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CM.Backend.API.RequestModels.UserRequestModels;
using System.Net.Http;
using CM.Backend.API.Helpers;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.ActionFilters;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Commands.Commands.NotificationCommands;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries.UserCreationQuerires;
using CM.Backend.Queries.Queries.UserQueries;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CM.Backend.API.Controllers
{
    public partial class UserController
    {
		/// <summary>
        /// Creates the user. An empty Usersettings object and userImageCustomization object is automatically added
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="createUser">Create user.</param>
        [HttpPost]
        [Route("")]
		[ServiceFilter(typeof(ClientValidationFilter))]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequestModel createUser)
        {
	        //Check if the email is still available <--
	        var emailAvailability = await QueryRouter.QueryAsync<CheckEmailAvailabillity, bool>(new CheckEmailAvailabillity(createUser.Email));
	        if (!emailAvailability)
	        {
		        return StatusCode(400, "An account with this email already exists");
	        }
	        
	        //Check if the username is still available <--
	        var usernameAvailability = await QueryRouter.QueryAsync<CheckUsernameAvailabillity, bool>(new CheckUsernameAvailabillity(createUser.Name));
	        if (!usernameAvailability)
	        {
		        return StatusCode(400, "An account with this username already exists");
	        }
	        
	        //Create User in backend.API
	        var userCreationResult = await CommandRouter.RouteAsync<CreateUser, IdResponse>(
		        new CreateUser(
			        createUser.Email.ToLower(),
			        createUser.Name,
			        createUser.ProfileName,
			        createUser.Biography,
			        createUser.UTFOffset));
	        
	        //****** //TODO : We could chain this in a transaction from the events <-- But we need httpClient knowledge to do this which we dont have from the context of an eventhandler.
	        //User created successfully in backend.API request a creation at the identityServer
	        //******

	        var baseurl = _identityConfig.Value.ConnectionString + "/identity-api/v1/users";
	        
	        //Prepare identityUserModel
            var identityJSON = new CreateIdentityUserModel
            {
	            Id = userCreationResult.Id,
                Email = createUser.Email.ToLower(),
                Password = createUser.Password
            };

            var httpClient = new HttpClient();

	        //Forward client authHeader and prepare request for application/json header
            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-Client-Id", Request.Headers["X-Client-Id"].ToString());
	        httpClient.DefaultRequestHeaders.Add("X-Client-Secret", Request.Headers["X-Client-Secret"].ToString());
	        
	        
            var httpContent = new StringContent(JsonConvert.SerializeObject(identityJSON, Formatting.Indented), System.Text.Encoding.UTF8, "application/json");

	        var attempts = 0;
	        var creationSuccesful = false;

	        HttpResponseMessage identityResponse = null;
	        
	        //While creationSuccesful is false we run the creation loop
	        while (!creationSuccesful)
	        {
		        //Ask for creation of user
		        identityResponse = await httpClient.PostAsync(baseurl, httpContent);
		        //If the response is SuccessStatusCode set creationSuccesfull to true to break the loop and keep running code.
		        if (identityResponse.IsSuccessStatusCode)
		        {
			        creationSuccesful = true;
		        }
		        else
		        {
			        //Since the creation failed wait 1 second or 1000 milliseconds, bump number of attempts and try creating again
			        await Task.Delay(1000);
			        attempts++;
			        //Attempt 0, 1, 2 failed if we at our 4th attempt break the loop and process failed code... Deleting the user again.
			        if (attempts >= 3)
			        {
				        break;
			        }
		        }
	        }
	        
            if (!identityResponse.IsSuccessStatusCode)
            {
                //The creation of the identityUser failed -->
	            //--> Rollback creation of user in the backend.
	            var deleteUserResult = await CommandRouter.RouteAsync<DeleteUser, Response>(new DeleteUser(userCreationResult.Id));
	            if (!deleteUserResult.IsSuccessful) //<-- Not good... the creation of identityUserFailed and we failed to delete the user again.
	            {
		            //We have now achieved error state since the user was neither succesfully created nor deleted again
		            //Inform user that shit has happended...
		            Logger.Fatal("Error: " + deleteUserResult.Message +
		                               ". --> Try again in abit or contact support. User creation process failed, and did not fully complete nor clean up. Failed with UID: " +
		                               identityJSON.Id +". {@CreateUserRequestModel}. Steps: 1 --> User created in backend succesfully; 2 --> Failed to create user on identityServer; 3 --> Failed to rollback creation of user in backend; 4 --> ErrorState! User should be deleted from backend", createUser);
		            return StatusCode(400,
			            "Error: " + deleteUserResult.Message +
			            ". --> Try again in abit or contact support. User creation process failed, and did not fully complete nor clean up. Failed with UID: " +
			            identityJSON.Id);
	            }
	            else
	            {
		            Logger.Fatal("Error: " + deleteUserResult.Message +
		                               ". --> UserCreation failed, failed to create user at identity server");
		            return StatusCode(400,
			            "Error: Contact Support, Creation of user failed");
	            }
            }

	        //Creation of identityUser was successful, return granted tokens.
	        var content = await identityResponse.Content.ReadAsStringAsync();
	         
            var tokenResponse = JsonConvert.DeserializeObject<IdentityUserCreated>(content);

            //Create welcome notification
            await CommandRouter.RouteAsync<UserCreatedNotification, Response>(new UserCreatedNotification(userCreationResult.Id));

            return new ObjectResult(tokenResponse);

        }
	    
	    [HttpPut]
	    [Route("currentUser/updateEmail")]
	    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
	    public async Task<IActionResult> UpdateEmail(string email, string password)
	    {
		    if (RequestingUserId.Equals(Guid.Empty))
		    {
			    return StatusCode(401);
		    }
		    
		    //Check that the email is not already being used
		    var result = await QueryRouter.QueryAsync<CheckEmailAvailabillity, bool>(new CheckEmailAvailabillity(email.ToLower()));
			if (!result)
		    {
			    return StatusCode(400, "This email is already being used by another account.");
		    }

		    //Gets the currentUser for rollback purposes
		    var user = await QueryRouter.QueryAsync<GetUser, UserQueryModel>(new GetUser(RequestingUserId));
		    
		    var baseurl = _identityConfig.Value.ConnectionString + "/identity-api/v1/users/" + RequestingUserId +
		                  "/updateEmail?email=" + email.ToLower() + "&password=" + password;
			
		    var client = new HttpClient();

		    var bearer = Request.Headers["Authorization"].ToString().Remove(0, 7);
			
		    client.SetBearerToken(bearer);

		    var response = await client.PutAsync(baseurl, null);
			
		    //The identityServer was not successfull in updating email... Return a message to the user.
		    if (!response.IsSuccessStatusCode)
		    {
			    if (response.Content != null)
			    {
				    var errorContent = await response.Content.ReadAsStringAsync();
				    if (string.IsNullOrEmpty(errorContent))
				    {
					    Logger.Fatal("Update email for {UserId} failed with response {errorMsg}. Did not update on identityServer", RequestingUserId, new ObjectResult(response));
					    return StatusCode((int)response.StatusCode, new ObjectResult(response));
				    }
				    Logger.Fatal("Update email for {UserId} failed with response {errorMsg}. Did not update on identityServer", RequestingUserId, errorContent);
					return StatusCode((int)response.StatusCode, errorContent);
			    }
			    Logger.Fatal("Update email for {UserId} failed with response {errorMsg}. Did not update on identityServer", RequestingUserId, new ObjectResult(response));
				return StatusCode((int)response.StatusCode, new ObjectResult(response));
		    }
			
		    //Email updated on identity-server! Update email here
		    var changeEmailResponse =
			    await CommandRouter.RouteAsync<UpdateUserEmail, Response>(new UpdateUserEmail(RequestingUserId, email.ToLower()));

		    //Changing email in backend failed <-- Rollback changes to the users previous email
		    if (!changeEmailResponse.IsSuccessful)
		    {
			    var reBaseUrl = _identityConfig.Value.ConnectionString + "/identity-api/v1/users/" + RequestingUserId +
			                    "/updateEmail?email=" + user.Email + "&password=" + password;
			    var reClient = new HttpClient();

			    var reBearer = Request.Headers["Authorization"].ToString().Remove(0, 7);
			
			    reClient.SetBearerToken(reBearer);

			    //Request the identity server to update email to previous email
			    HttpResponseMessage reResponse = null;
			    var attempts = 0;
			    var successfulAttempt = false;

			    //Make three attempts of rolling back the error state we are in
			    while (!successfulAttempt)
			    {
				    reResponse = await reClient.PutAsync(reBaseUrl, null);

				    if (reResponse.IsSuccessStatusCode)
				    {
					    successfulAttempt = true;
				    }
				    else
				    {
					    attempts++;
					    await Task.Delay(100);
					    if (attempts >= 3)
					    {
						    break;
					    }
				    }
			    }
				  
			    //The request to rollback failed! This means we are in error state!
			    //1. User updated on identityServer <-- Success
			    //2. User failed to updated in backend <-- Error
			    //3. Failed to rollback identityServer <-- Error
			    //Result : The IdentityServer user has a different email than the backend.User
			    if (!reResponse.IsSuccessStatusCode)
			    {
				    Logger.Fatal("Updating email for {UserId} failed... 1 --> Email was updated in identityServer; 2 --> Failed to update email in backend; 3 --> Failed to rollback identityServer to previous email; 4 --> Error state the backend email was changed but identityServer was not rolledBack", RequestingUserId);
				    return StatusCode(400, "Error: " + changeEmailResponse.Message + ". --> Updating email failed in process, while updating email:" + user.Email + " to" + email.ToLower() + ". Try again in a bit or contact support.");
			    }
			    
			    //Rollback was succesfull, which means that we avoided error state. --> But the updated was still not succesfull
			    Logger.Fatal("Updating email for {UserId} failed... 1 --> Email was updated in identityServer; 2 --> Failed to update email in backend; 3 --> Rolled back email in identityServer;", RequestingUserId);
				return StatusCode(400, changeEmailResponse.Message);
		    }
		    return StatusCode(200);
	    }

		[HttpGet]
		[Route("usernameavailabillity")]
		[ServiceFilter(typeof(ClientValidationFilter))]
		public async Task<IActionResult> UsernameAvailabillity(string username)
		{
			var result = await QueryRouter.QueryAsync<CheckUsernameAvailabillity, bool>(new CheckUsernameAvailabillity(username));

			if(result)
			{
				return StatusCode(200, true);
			}

			return StatusCode(400, "Username is already being used");
		}

		[HttpGet]
        [Route("emailavailabillity")]
		[ServiceFilter(typeof(ClientValidationFilter))]
        public async Task<IActionResult> EmailAvailabillity(string email)
		{
			var result = await QueryRouter.QueryAsync<CheckEmailAvailabillity, bool>(new CheckEmailAvailabillity(email.ToLower()));
        
			if (result)
            {
                return StatusCode(200, true);
            }

            return StatusCode(400, "This email is already being used");
		}
    }
}
