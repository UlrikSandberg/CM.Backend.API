using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CM.Backend.API.ActionFilters;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.Helpers;
using CM.Backend.API.RequestModels.CommentRequestModels;
using CM.Backend.Commands.Commands.CommentsCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Queries.Model.CommentModels;
using CM.Backend.Queries.Queries.TastingQueries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Schema;
using Serilog;

namespace CM.Backend.API.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/comments")]
    [ServiceFilter(typeof(UnifiedEndpointNameFilter))]
    [Authorize(Policy = nameof(AuthorizationRoles.CMUser))]
    public class CommentsController : ControllerBase
    {
        public CommentsController(ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<IdentityServerConfiguration> identityConfig, ILogger logger) : base(commandRouter, queryRouter, identityConfig, logger)
        {
        }


        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("")]
        public async Task<IActionResult> CreateComment([Microsoft.AspNetCore.Mvc.FromBody]CreateCommentRequestModel requestModel)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response = await CommandRouter.RouteAsync<CreateComment, IdResponse>(new CreateComment(requestModel.ContextId,
                requestModel.ContextType.ToString(), RequestingUserId, requestModel.Date, requestModel.Comment));

            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            var result =
                await QueryRouter.QueryAsync<GetComment, CommentModel>(new GetComment(response.Id, RequestingUserId));

            if (result == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(result);

        }

        [Microsoft.AspNetCore.Mvc.HttpPut]
        [Microsoft.AspNetCore.Mvc.Route("{commentId}")]
        public async Task<IActionResult> EditComment(Guid commentId, [Microsoft.AspNetCore.Mvc.FromBody] EditCommentRequestModel requestModel)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response =
                await CommandRouter.RouteAsync<EditComment, Response>(new EditComment(commentId, RequestingUserId,
                    requestModel.Content));


            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            return StatusCode(201);

        }

        [Microsoft.AspNetCore.Mvc.HttpDelete]
        [Microsoft.AspNetCore.Mvc.Route("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid commentId)
        {
            if (RequestingUserId.Equals(Guid.Empty))
            {
                return StatusCode(401);
            }

            var response =
                await CommandRouter.RouteAsync<DeleteComment, Response>(new DeleteComment(commentId, RequestingUserId));

            if (!response.IsSuccessful)
            {
                return StatusCode(400, response.Message);
            }

            return StatusCode(201);

        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("")]
        [ResponseCache(Duration = 60 * 60, VaryByQueryKeys = new []{"contextId", "page", "pageSize", "orderAcendingByDate"})]
        public async Task<IActionResult> GetCommentsForContextId(Guid contextId, int page, int pageSize,
            bool orderAcendingByDate = false)
        {
            var result =
                await QueryRouter.QueryAsync<GetComments, IEnumerable<CommentModel>>(new GetComments(RequestingUserId,
                    contextId, page, pageSize, orderAcendingByDate));

            if (result == null)
            {
                return NotFound();
            }
            
            return new ObjectResult(result);
        }
    }
}