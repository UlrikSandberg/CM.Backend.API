using System;
using System.Net.Http.Headers;
using CM.Backend.API.Helpers;
using CM.Backend.Messaging.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using CM.Backend.API.ActionFilters;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace CM.Backend.API.Controllers
{
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    public class ControllerBase : Controller
    {
        protected readonly ICommandRouter CommandRouter;
        protected readonly IQueryRouter QueryRouter;
        protected readonly ILogger Logger;
        
        protected Guid RequestingUserId
        {
            get
            {
                AuthenticationHeaderValue.TryParse(HttpContext.Request.Headers[HeaderNames.Authorization], out var parsedValue);

                if (parsedValue == null)
                    return Guid.Empty;
            
                //Decrypt token
                var handler = new JwtSecurityTokenHandler();
                var decodedToken = handler.ReadJwtToken(parsedValue.Parameter);

                if (decodedToken == null) return Guid.Empty;
            
                Guid.TryParse(decodedToken.Subject, out var parsedId);

                return parsedId;
            }
        }

        public ControllerBase(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger)
        {
            Logger = logger;
            CommandRouter = commandRouter;
            QueryRouter = queryRouter;
        }
    }
}
