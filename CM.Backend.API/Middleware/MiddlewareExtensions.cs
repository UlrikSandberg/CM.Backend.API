using CM.Backend.API.ActionFilters;
using CM.Backend.API.Middleware.CorrelationMiddleware;
using CM.Backend.API.Middleware.GlobalExceptionFilter;
using CM.Backend.API.Middleware.ServiceInfo;
using Microsoft.AspNetCore.Builder;

namespace CM.Backend.API.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDefaultCMLoggingMiddlewares(this IApplicationBuilder builder)
        {
            return builder
                .UseMiddleware<GlobalExceptionMiddleware>()
                .UseMiddleware<CorrelationIdMiddleware>()
                .UseMiddleware<ServiceInfoLoggingMiddleware>()
                .UseMiddleware<RequestResponseLoggingMiddleware>()
                .UseMiddleware<IPLoggingMiddleware>();
        }
    }
}