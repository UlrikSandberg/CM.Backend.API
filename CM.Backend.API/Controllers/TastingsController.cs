using System;
using System.Threading.Tasks;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.Helpers;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Queries.Model.TastingModels;
using CM.Backend.Queries.Queries.TastingQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

namespace CM.Backend.API.Controllers
{
    [Route("api/v1/tastings")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
    public class TastingsController : ControllerBase
    {
        public TastingsController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
        {
        }

        [HttpGet]
        [Route("{tastingId}")]
        public async Task<IActionResult> GetTasting(Guid tastingId)
        {
            var result =
                await QueryRouter.QueryAsync<GetChampagneTasting, TastingModel>(
                   new GetChampagneTasting(tastingId, RequestingUserId));

            if (result == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(result);

        }

        [HttpGet]
        [Route("{tastingId}/inspectTasting")]
        public async Task<IActionResult> GetInspectTasting(Guid tastingId, int page, int pageSize, bool acendingOrderByDate = false)
        {
            var result = await QueryRouter.QueryAsync<GetInspectTasting, InspectTastingModel>(
                new GetInspectTasting(RequestingUserId, tastingId, page, pageSize, acendingOrderByDate));

            if (result == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(result);

        }
    }
}