using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using CM.Backend.API.ActionFilters;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CM.Backend.API.Controllers
{
    [Route("api/v1/health")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    public class MonitorController : Controller
    {
        private readonly IQueryRouter _queryRouter;

        public MonitorController(IQueryRouter queryRouter)
        {
            _queryRouter = queryRouter;
        }
        
        /// <summary>
        /// Perform a shallow health check
        /// </summary>
        /// <returns></returns>
        [Route("shallow")]
        [HttpGet]
        public async Task<IActionResult> Shallow()
        {
            return Ok();
        }

        /// <summary>
        /// Perform a deep health check, verifying dependent services
        /// </summary>
        /// <returns></returns>
        [Route("deep")]
        [HttpGet]
        public async Task<IActionResult> Deep()
        {
            var result = await _queryRouter.QueryAsync<GetAllBrands, IEnumerable<BrandLight>>(new GetAllBrands(0, 1, true, true));
            
            return result == null ? StatusCode(500) : Ok();
        }
    }
}