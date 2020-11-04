using System;
using System.Threading.Tasks;
using CM.Backend.Documents.GlobalLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace CM.Backend.API.ActionFilters
{
    /// <summary>
    /// Push the respective actions routeTemplate to the logContext.
    /// This way we can correlate execution time on URI's containing
    /// {id} in the route signature. These URI's are otherwise considered
    /// different in the logging service thus making it difficult to correlate
    /// execution times. This logContext should make it easy to correlate the
    /// respective URI's
    /// </summary>
    public class UnifiedEndpointNameFilter : ActionFilterAttribute
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UnifiedEndpointNameFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var actionDescriptor = context.ActionDescriptor;

            var methodRouteTemplate = actionDescriptor.AttributeRouteInfo.Template;
            
            SharedLoggingProperties.AddMethodRouteTemplate(_httpContextAccessor.HttpContext.TraceIdentifier, methodRouteTemplate);
            
            await base.OnActionExecutionAsync(context, next);
        }
    }
}