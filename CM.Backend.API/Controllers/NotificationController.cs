using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.Helpers;
using CM.Backend.API.RequestModels.UserRequestModels;
using CM.Backend.Commands.Commands.NotificationCommands;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.EventHandlers.Helpers;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Model.NotificationModels;
using CM.Backend.Queries.Model.UserModels;
using CM.Backend.Queries.Queries;
using CM.Backend.Queries.Queries.NotificationQueries;
using CM.Backend.Queries.Queries.UserQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleSoft.Mediator;
using SimpleSoft.Mediator.Pipeline;
using ILogger = Serilog.ILogger;
using NotificationTags = CM.Backend.API.EnumOptions.NotificationTags;

namespace CM.Backend.API.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/notifications")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
    public class NotificationController : ControllerBase
    {
        private readonly IOptions<NotificationHubConfiguration> notificationsOptions;

        
        public NotificationController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, IOptions<NotificationHubConfiguration> notificationsOptions, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
        {
            this.notificationsOptions = notificationsOptions;
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("currentUser/markLatestNotificationSeen/{notificationId}")]
        public async Task<IActionResult> MarkLatestNotificationSeen(Guid notificationId)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response =
                await CommandRouter.RouteAsync<MarkLatestNotificationSeen, Response>(
                    new MarkLatestNotificationSeen(RequestingUserId, notificationId));

            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            return StatusCode(200);

        }
        
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("currentUser/notifications/getLatest")]
        public async Task<IActionResult> GetLatestNotifications(bool includeUpdates, Guid fromId)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            if (includeUpdates && fromId.Equals(Guid.Empty))
            {
                return StatusCode(400, "If includeUpdates is true, a valid fromId must be provided");
            }

            var response = await QueryRouter.QueryAsync<GetLatestNotificationUpdateForUserAsync, LatestNotificationUpdateModel>(new GetLatestNotificationUpdateForUserAsync(RequestingUserId, includeUpdates, fromId));

            if (response == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(response);
        }
        
        
        /// <summary>
        /// Mark a notification as read for a specific user. Authenticated with a bearer token.
        /// </summary>
        /// <param name="notificationId">Id of notification to be marked as read.</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("currentUser/notifications/markAsRead/{notificationId}")]
        public async Task<IActionResult> FlagNotificationAsRead(Guid notificationId)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response = await CommandRouter.RouteAsync<MarkNotificationAsRead, Response>(new MarkNotificationAsRead(RequestingUserId, notificationId));

            if (!response.IsSuccessful)
            {
                return StatusCode(400, response);
            }

            return StatusCode(200);
        }
        
        /// <summary>
        /// Get notifications paged async
        /// </summary>
        /// <param name="page">Which page are we on.</param>
        /// <param name="pageSize">Define the size of each page.</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("currentUser/notifications")]
        public async Task<IActionResult> GetNotificationsForUserPagedAsync(int page, int pageSize)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var result =
                await QueryRouter.QueryAsync<GetNotificationsPagedAsync, IEnumerable<NotificationQueryModel>>(
                    new GetNotificationsPagedAsync(RequestingUserId, page, pageSize));

            if (result == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(result);
        }
        
        /// <summary>
        /// Use deregister a currently registered device installation of the respective user asking for deregistering.
        /// </summary>
        /// <param name="deviceInstallationId">The id of the respective deviceInstallation</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("currentUser/deviceInstallations/{deviceInstallationId}")]
        public async Task<IActionResult> DeregisterDevice(string deviceInstallationId)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response = await CommandRouter.RouteAsync<DeregisterDeviceInstallation, Response>(new DeregisterDeviceInstallation(RequestingUserId, deviceInstallationId));

            if (!response.IsSuccessful)
            {
                return StatusCode(400);
            }

            return StatusCode(200);
        }
        
        /// <summary>
        /// Use to register a device installation with a user.
        /// </summary>
        /// <param name="deviceInstallation">Device installtion details, to be stored in respect to the user uploading through bearer token.</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("currentUser/deviceInstallations")]
        public async Task<IActionResult> RegisterDevice([Microsoft.AspNetCore.Mvc.FromBody]DeviceInstallationRequestModel deviceInstallation)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }
            
            //Validate that the provided notificationPlatform is valid
            if (!StaticResources.ValidateNotificationPlatform(deviceInstallation.Platform))
            {
                Logger.Error("Provided notification platform: " + deviceInstallation.Platform +
                ". Is not supported only -> mpns, wns, apns, gcm, fcm are valid providers");
                return StatusCode(400,
                    "Provided notification platform: " + deviceInstallation.Platform +
                    ". Is not supported only -> mpns, wns, apns, gcm, fcm are valid providers");
            }
            
            //Register deviceTimeZone -->
            var timeZoneUpdate =
                await CommandRouter.RouteAsync<UpdateUserTimeZone, Response>(
                    new UpdateUserTimeZone(RequestingUserId, deviceInstallation.UTCOffset));
            
            //Get currentUser!
            var user = await QueryRouter.QueryAsync<GetPersistenceUserModel, User>(new GetPersistenceUserModel(RequestingUserId));

            var isAlreadyRegistered = false;
            //Check if device has already been registered if so just update notification hub with the information
            if (user != null)
            {
                if (user.DeviceInstallations != null)
                {
                    foreach (var installation in user.DeviceInstallations)
                    {
                        if (installation.PushChannel.Equals(deviceInstallation.PushChannel))
                        {
                            isAlreadyRegistered = true;
                            break;
                        }
                    }
                }
            }

            if (isAlreadyRegistered)//Since device has already been registered make sure notification hub is up to date.
            {
                await UpdateNotificationHub(user, deviceInstallation.PushChannel);

                return StatusCode(200);
            }
            else //Since the device has not been registered invoke command to register device with user
            {
                var response = await CommandRouter.RouteAsync<RegisterDeviceInstallation, Response>(new RegisterDeviceInstallation(RequestingUserId, deviceInstallation.PushChannel, deviceInstallation.Platform));

                return StatusCode(!response.IsSuccessful ? 403 : 200);
            }
        }

        private async Task UpdateNotificationHub(User user, string pushChannel)
        {
            Installation installation = null;
            foreach (var deviceInstallation in user.DeviceInstallations)
            {
                if (!deviceInstallation.PushChannel.Equals(pushChannel)) continue;
                installation = deviceInstallation;
                break;
            }
            
            //Update notification hub with the current settings
            var tags = new List<string>();

            if (user.Notifications != null)
            {
                if (user.Notifications.ReceiveCMNotifications)
                {
                    tags.Add(NotificationTags.CMNotification.ToString());
                }
            }
            
            //Update notification hubs
            var client = NotificationHubClient.CreateClientFromConnectionString(
                notificationsOptions.Value.NotificationHubConnectionString,
                notificationsOptions.Value.NotificationHubName);
            installation.Tags = tags;

            await client.CreateOrUpdateInstallationAsync(installation);
        }
        
        /// <summary>
        /// Send community updates! Be aware that the push-notification will have content of the broadCastMessage and if left empty a notification in app is the only thing they will see. Respect the different placeholder settings for message as well as for broadcasting.
        /// </summary>
        /// <param name="message">The messages of the notification, should include placeholders like {InvokedByName} and {BrandName}...</param>
        /// <param name="broadCastMessage">The message that will be shown as a push-notification. Does not support {InvokedByName} placeholder. For placeholder and notification content in app use message</param>
        /// <param name="contextUrl">The contextUrl of this notification, in order to route user in app. Needs to be provided if you want to omit insert '/'</param>
        /// <param name="brandId">Can be left empty as 00000000-0000-0000-0000-000000000000. If not empty the API will look for a BrandName placeholder -> {BrandName} and fill with the name of the respective brand, if no valid placeholder is given the call will fail</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("sendCommunityNotification")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> SendCommunityNotification(bool useMessagePlaceholder, string message, string broadCastMessage, string contextUrl, Guid brandId)
        {
            if (!contextUrl.EndsWith("/"))
            {
                return StatusCode(400, "The provided contextUrl must end with a /");
            }
            
            //Only validate invokedbyName if useMessagePlaceholder is active
            if (useMessagePlaceholder)
            {
                if (!message.Contains("{InvokedByName}"))
                {
                    return StatusCode(400,
                        "The provided messages does not contain a valid Name placeholder. Correct usage -> {InvokedByName}");
                }
            }

            if (!brandId.Equals(Guid.Empty))
            {
                if (!message.Contains("{BrandName}"))
                {
                    return StatusCode(400,
                        "The provided messages does not contain valid BrandName placeholder. Correct usage -> {BrandName}");
                }
            }

            if (broadCastMessage.Contains("{InvokedByName}"))
            {
                return StatusCode(400,
                    "The provided broadcast messages does not support NamePlaceholder -> {InvokedByName}");
            }

            var response =
                await CommandRouter.RouteAsync<CommunityUpdateNotification, Response>(
                    new CommunityUpdateNotification(message, broadCastMessage, contextUrl, brandId));

            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            return StatusCode(200);
        }

        /// <summary>
        /// Send brand news!
        /// </summary>
        /// <param name="brandId">Id of the respective champagne name will be fetch and replace {InvokedByName}</param>
        /// <param name="message">The message of the notifications, should show placeholders like {InvokedByName}, {ChampagneName}</param>
        /// <param name="contextUrl">The contextUrl of this notification, in order to route user in app</param>
        /// <param name="champagneId">Can be left empty as 00000000-0000-0000-0000-000000000000. If not empty the API will look for the {ChampagneName} placeholder and fill with the name of the respective champagne</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("sendBrandNewsNotification")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> SendBrandNewsNotification(Guid brandId, string message, string contextUrl, Guid champagneId)
        {
            //Validate that the contextUrl given is valid
            if (!contextUrl.EndsWith('/'))
            {
                return StatusCode(400, "The provided contextUrl must end with a /");
            }
            
            var brand =
                await QueryRouter.QueryAsync<GetBrand, BrandProfileExtendedBrandPage>(new GetBrand(brandId,
                    Guid.Empty));

            if (brand == null)
            {
                return StatusCode(400, "Provided brandId is invalid");
            }
            
            //Determine if the name placeholder is written properly
            if (!message.Contains("{InvokedByName}"))
            {
                return StatusCode(400, "The provided messages does not contain a valid NamePlaceholder. Correct usage -> {InvokedByName}");
            }

            var response = await CommandRouter.RouteAsync<BrandNewsNotification, Response>(
                new BrandNewsNotification(brand.Id, brand.Name, brand.LogoImageId, message, contextUrl, champagneId));

            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            return StatusCode(200);
        }

        /// <summary>
        /// Create a contextUrl for brands!
        /// </summary>
        /// <param name="newsType"></param>
        /// <param name="brandId"></param>
        /// <param name="contextId"></param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("createbrandContextUrl")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> CreateBrandContextUrl(BrandNotificationNews newsType, Guid brandId, Guid contextId)
        {
            if (newsType.Equals(BrandNotificationNews.BrandInfoPage))
            {
                return new ObjectResult(EventHandlers.Helpers.urlRoot.brand + "/" + brandId + "/" + EventHandlers.Helpers.urlSpecification.page + "/" + contextId + "/");
            }

            if (newsType.Equals(BrandNotificationNews.ChampagneUpdate))
            {
                return new ObjectResult(EventHandlers.Helpers.urlRoot.champagne + "/" + contextId + "/");
            }

            return new ObjectResult("");
        }

        /// <summary>
        /// This can only autogenerate contextUrl for new brands given that brandId is provided
        /// </summary>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("createCommunityContextUrl")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> CreateCommunityContextUrl(Guid brandId)
        {
            var brand =
                await QueryRouter.QueryAsync<GetBrand, BrandProfileExtendedBrandPage>(new GetBrand(brandId,
                    Guid.Empty));

            if (brand == null)
            {
                return StatusCode(400, "Provided brandId is invalid");
            }
            
            var contextUrl = new StringBuilder();

            contextUrl.Append(CM.Backend.EventHandlers.Helpers.urlRoot.brand);

            contextUrl.Append("/");

            contextUrl.Append(brandId);

            contextUrl.Append("/");

            contextUrl.Append(CM.Backend.EventHandlers.Helpers.urlSpecification.profile);

            contextUrl.Append("/");

            return new ObjectResult(contextUrl.ToString());

        }
        
        /// <summary>
        /// Use to create custom contextUrls be aware that these URLs are not checked for validility. Use with caution and respect to the conventions
        /// </summary>
        /// <param name="urlRoot">The urlRoot with 4 parameters, namely -> champagne, brand, user, publicuser. Each parameter has a set of valid urlNiveau1 arguments which internally have a set of valid urlNiveau2 arguments
        /// Niveau 1 champagne arguments -> None, inspecttasting.
        /// Niveau 1 brand arguments     -> profile, cellar, page.
        /// Niveau 1 user arguments      -> cellar.
        /// Niveau 1 publicUser arguments-> None, cellar</param>
        /// <param name="urlRootPath">The id path for respective urlRoot</param>
        /// <param name="urlNiveau1">Niveau 1 arguments, have respect for the conventions.</param>
        /// <param name="urlNiveau1Path">The id path for respective urlNiveau1</param>
        /// <param name="urlNiveau2">Niveau 2 arguments, have respect for the conventions</param>
        /// <param name="urlNiveau2Path">The id path for respective urlNiveau2</param>
        /// <returns></returns>
        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("createCustomContextUrl")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
        public async Task<IActionResult> CreateCustomContextUrl(CM.Backend.EventHandlers.Helpers.urlRoot urlRoot, Guid urlRootPath, CM.Backend.EventHandlers.Helpers.urlSpecification urlNiveau1, Guid urlNiveau1Path, CM.Backend.EventHandlers.Helpers.urlSpecification urlNiveau2, Guid urlNiveau2Path)
        {
            if (urlRootPath.Equals(Guid.Empty))
            {
                return new ObjectResult(null);
            }
            
            var contextUrl = new StringBuilder();

            contextUrl.Append(urlRoot);

            contextUrl.Append("/");

            contextUrl.Append(urlRootPath);

            contextUrl.Append("/");

            if (urlNiveau1.Equals(CM.Backend.EventHandlers.Helpers.urlSpecification.unknown))
            {
                return new ObjectResult(contextUrl.ToString());
            }

            if (urlNiveau1Path.Equals(Guid.Empty))
            {
                return new ObjectResult(null);
            }

            contextUrl.Append(urlNiveau1);

            contextUrl.Append("/");

            contextUrl.Append(urlNiveau1Path);

            contextUrl.Append("/");

            if (urlNiveau2.Equals(CM.Backend.EventHandlers.Helpers.urlSpecification.unknown))
            {
                return new ObjectResult(contextUrl.ToString());
            }

            if (urlNiveau2Path.Equals(Guid.Empty))
            {
                return new ObjectResult(null);
            }
            
            contextUrl.Append(urlNiveau2);

            contextUrl.Append("/");

            contextUrl.Append(urlNiveau2Path);

            contextUrl.Append("/");

            return new ObjectResult(contextUrl.ToString());

        }
    }
}