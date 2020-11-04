using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.Helpers;
using CM.Backend.API.Views.ConfirmEmail;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;

namespace CM.Backend.API.Controllers
{
    [Route("api/v1/confirmEmail")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    public class ConfirmEmailController : ControllerBase
    {
        public ConfirmEmailController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
        {
        }

        [HttpGet]
        [Route("{token}")]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            //Check if token match any confirmation process
            var result = await QueryRouter.QueryAsync<GetEmailConfirmationProcess, EmailConfirmationProcess>(new GetEmailConfirmationProcess(token));
            
            if (result == null)
            {
                return View("ConfirmEmail", new ConfirmEmailResponseModel
                {
                    IsSuccesfull = false,
                    Message = "Invalid email confirmation link. Try requesting a new confirmation email from the app."
                });
            }

            if (!result.IsActive)
            {
                Logger.Error("Confirmation Process failed, due to inactive email link, {@ProcesState}", result);
                
                return View("ConfirmEmail", new ConfirmEmailResponseModel
                {
                    IsSuccesfull = false,
                    Message = "This confirmation email link is no longer active. The link might already have been used once... Try requesting a new confirmation email from the app."
                });
            }

            var expirationDate = result.ConfirmationEmailInitiatedAt.AddDays(1);

            if (DateTime.UtcNow.CompareTo(expirationDate) > 0)
            {
                Logger.Error("Confirmation Process failed, due to expired email link, {@ProcesState}", result);
                
                return View("ConfirmEmail", new ConfirmEmailResponseModel
                {
                    IsSuccesfull = false,
                    Message = "The confirmation email link has expired. Request a new confirmation email from the app"
                });
            }
            
            //Confirm the email
            var response =
                await CommandRouter.RouteAsync<ConfirmEmail, Response>(new ConfirmEmail(result.UserId, result.Email));

            if (!response.IsSuccessful)
            {
                Logger.Error("Confirmation Process failed, due to expired email link, {@ProcesState}, {@Message}", result, response.Message);
                
                return View("ConfirmEmail", new ConfirmEmailResponseModel
                {
                    IsSuccesfull = false,
                    Message = "Something went wrong, there might be a mismatch between the email to be confirmed and the current email of the account"
                });
            }

            return View("ConfirmEmail", new ConfirmEmailResponseModel
            {
                IsSuccesfull = true
            });
        }

        [HttpPost]
        [Route("resendconfirmationemail")]
        [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
        public async Task<IActionResult> ResendConfirmationEmail()
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response =
                await CommandRouter.RouteAsync<ResendConfirmationEmail, Response>(new ResendConfirmationEmail(RequestingUserId));


            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            return StatusCode(200);
        }
    }
}