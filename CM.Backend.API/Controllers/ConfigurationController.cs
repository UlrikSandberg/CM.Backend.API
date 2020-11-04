using System.Threading.Tasks;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.RequestModels;
using CM.Backend.Queries.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CM.Backend.API.Controllers
{
    [ServiceFilter(typeof(ClientValidationFilter))]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    [Route("api/v1/configuration")]
    public class ConfigurationController : Controller
    {
        private readonly IOptions<AppConfiguration> _config;

        public ConfigurationController(IOptions<AppConfiguration> config)
        {
            _config = config;
        }
        
        /// <summary>
        /// Get the environment specific configuration for the App
        /// The endpoint returns configuration for the current environment in which the endpoint is called
        /// </summary>
        /// <returns>A configuration file with environment specific resources</returns>
        [HttpGet]
        [Route("")]
        [ResponseCache(Duration = 60 * 60 * 24)]
        public async Task<IActionResult> GetConfiguration()
        {
            return Ok(_config.Value);
        }
    }
}