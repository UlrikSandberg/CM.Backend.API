using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CM.Backend.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Serilog;

namespace CM.Backend.API.ActionFilters
{
    public class ClientValidationFilter : ActionFilterAttribute
    {
        private readonly IOptions<IdentityServerConfiguration> _identityConfig;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        public ClientValidationFilter(IOptions<IdentityServerConfiguration> identityConfig, IMemoryCache cache, ILogger logger)
        {
            _identityConfig = identityConfig;
            _cache = cache;
            _logger = logger;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authorized = await Authorize(context);

            if (!authorized)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await base.OnActionExecutionAsync(context, next);
        }

        private async Task<bool> Authorize(ActionContext context)
        {
            var clientId = context.HttpContext.Request.Headers.FirstOrDefault(x =>
                x.Key.Equals("X-Client-Id", StringComparison.OrdinalIgnoreCase));
            var clientSecret = context.HttpContext.Request.Headers.FirstOrDefault(x =>
                x.Key.Equals("X-Client-Secret", StringComparison.OrdinalIgnoreCase));
              
            if (!clientId.Value.Any() && !clientSecret.Value.Any())
            {
                _logger.Information("Tried to authorize client, but required headers was not present");
                return false;
            }

            var sw = new Stopwatch();
            sw.Start();
            var isValid = await _cache.GetOrCreateAsync($"{clientId.Value.First()}-{clientSecret.Value.First()}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("X-Client-Id", clientId.Value.First());
                    client.DefaultRequestHeaders.Add("X-Client-Secret", clientSecret.Value.First());

                    var result =
                        await client.GetAsync(_identityConfig.Value.ConnectionString + "/identity-api/v1/Validation");

                    if (!result.IsSuccessStatusCode)
                        _logger.Warning("Client authorization using {ClientId}/{ClientSecret} was unsuccessful. Identity served said {HttpStatusCode}. Rejecting..", clientId.Value.First(), clientSecret.Value.First(), (int)result.StatusCode);

                    return result.IsSuccessStatusCode;
                }
            });
            sw.Stop();
            _logger.Information("Authorization check took {ElapsedMillis}", sw.ElapsedMilliseconds);
            
            if(!isValid)
                _logger.Warning("Invocation from unauthorized {ClientId}/{ClientSecret}. Rejecting..", clientId.Value.First(), clientSecret.Value.First());
            
            return isValid;
        }
    }
}