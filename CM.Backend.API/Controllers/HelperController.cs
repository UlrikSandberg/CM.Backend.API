using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.Helpers;
using CM.Backend.API.RequestModels;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model;
using CM.Backend.Queries.HelperQueries;
using CM.Backend.Queries.Queries.HelperQueries;
using CM.Backend.Queries.Queries.HelperQueries.HelperModels;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Serilog;
using SimpleSoft.Mediator;

namespace CM.Backend.API.Controllers
{
    
    [Route("api/v1/helpers")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    [Authorize(Policy = nameof(AuthorizationRoles.CMAdmin))]
    public class HelperController : ControllerBase
    {
        
        public HelperController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
        {
        }

        [HttpGet]
        [Route("ChampagneProfileFilterSearchBuilder")]
        public async Task<IActionResult> FilterSearchParameterBuilder(DosageEnum.Dosage dosage,
            ChampagneStyleEnum.Style style1)
        {

            var styleList = new List<ChampagneStyleEnum.Style>();

            if (style1 != null)
            {
                styleList.Add(style1);
            }
            
            var filterSearchParameters = new AddChampagneFilterSearchOptionRequest();
            filterSearchParameters.Dosage = null;
            filterSearchParameters.Styles = new List<string>();
            filterSearchParameters.Characters = new List<string>();
            
            //Add Dosage
            if (dosage != null)
            {
                filterSearchParameters.Dosage = DosageEnum.DosageEnumConverter(dosage);
            }
            
            foreach (var style in styleList.Distinct())
            {
                filterSearchParameters.Styles.Add(ChampagneStyleEnum.StyleEnumConvert(style));
            }
            
            
            return new ObjectResult(filterSearchParameters);
        }

        /// <summary>
        ///
        /// Returns a list of all submitted feedback based on a few parameters, such as page, pageSize and the date range of the query
        ///
        /// Pay Attention to the dateTime formatting explained at fromDate and toDate. toDate must be later than fromDate and inverse.
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="fromDate">mm/dd/yyyy 00:00:00 : Example --> 05/29/2015 05:50:45</param>
        /// <param name="toDate">mm/dd/yyyy 00:00:00 : Example --> 05/29/2015 05:50:45</param>
        /// <returns></returns>
        [HttpGet]
        [Route("FeedBack")]
        public async Task<IActionResult> GetSubmittedFeedback(int page, int pageSize, DateTime fromDate, DateTime toDate)
        {
            var result =
                await QueryRouter.QueryAsync<GetSubmittedFeedbackQuery, IEnumerable<BugAndFeedback>>(
                    new GetSubmittedFeedbackQuery(page, pageSize, fromDate, toDate));

            if (result == null)
            {
                return NotFound();
            }
                
            return new ObjectResult(result);  
        }

        /// <summary>
        /// Returns a list of all emails based on a few parameters, such as page, pageSize and the date range of the query
        ///
        /// Pay Attention to the dateTime formatting explained at fromDate and toDate. toDate must be later than fromDate and inverse.
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="fromDate">mm/dd/yyyy 00:00:00 : Example --> 05/29/2015 05:50:45</param>
        /// <param name="toDate">mm/dd/yyyy 00:00:00 : Example --> 05/29/2015 05:50:45</param>
        /// <param name="isVerbose">Indicates whether the list contains extra information. True=More information, False=less information</param>
        /// <returns></returns>
        [HttpGet]
        [Route("EmailList")]
        public async Task<IActionResult> GetEmailList(int page, int pageSize, DateTime fromDate, DateTime toDate, bool isVerbose = false)
        {
            object result = null;

            if (isVerbose)
            {
                result = await QueryRouter.QueryAsync<GetEmailListVerbose, IEnumerable<VerboseEmailListModel>>(
                    new GetEmailListVerbose(page, pageSize, fromDate, toDate));
            }
            else
            {
                result = await QueryRouter.QueryAsync<GetEmailList, IEnumerable<string>>(
                    new GetEmailList(page, pageSize, fromDate, toDate));
            }

            if (result == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(result);
        }


        /// <summary>
        /// After the new ratingModel has been introduced we will have to copy each rating from every champagne into the ratingRepository
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("MigrateChampagneRatings")]
        public async Task<IActionResult> MigrateChampagneRatings()
        {
            var result =
                await QueryRouter.QueryAsync<MigrateChampagneRatings, IEnumerable<Guid>>(new MigrateChampagneRatings());

            if (result == null)
            {
                return NotFound();
            }

            return new ObjectResult(result);
        }

        [HttpPut]
        [Route("UpdateChampagneRatingStatus")]
        public async Task<IActionResult> UpdateChampagneRatingStatus(bool updateStatus)
        {
            var result =
                await QueryRouter.QueryAsync<UpdateChampagneRatingStatus, bool>(
                    new UpdateChampagneRatingStatus(updateStatus));

            if (result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPut]
        [Route("UpdateChampagneFolderDiscoverVisibility")]
        public async Task<IActionResult> UpdateChampagneFolderDiscoverVisibility()
        {
            var result =
                await QueryRouter.QueryAsync<UpdateChampagneFolderWithDiscoverVisibilityBoolean, bool>(
                    new UpdateChampagneFolderWithDiscoverVisibilityBoolean());

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmds">Clear the champagneSearch cache by creating an entry with this key: GetAllBrandsCached</param>
        /// <returns></returns>
        [HttpPut]
        [Route("ClearInMemoryCache")]
        public async Task<IActionResult> ClearInMemoryCache(List<string> cmds)
        {
            var result =
                await QueryRouter.QueryAsync<ClearInMemoryCache, bool>(new ClearInMemoryCache(cmds));

            if (!result)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
