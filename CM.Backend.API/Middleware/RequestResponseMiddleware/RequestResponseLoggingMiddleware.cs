using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CM.Backend.API.Helpers;
using CM.Backend.Documents.GlobalLogging;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;

namespace CM.Backend.API.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<ServiceInfoConfiguration> _serviceInfo;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger logger, IOptions<ServiceInfoConfiguration> serviceInfo)
        {
            _next = next;
            _logger = logger;
            _serviceInfo = serviceInfo;
        }
        
        public async Task Invoke(HttpContext context)
        {
            SharedLoggingProperties.CreateSharedContextEntry(context.TraceIdentifier);
            
            context.Response.OnStarting(state =>
            {
                var ctx = (HttpContext) state;
                ctx.Response.Headers.Add("x-build-id", _serviceInfo.Value.BuildId);
                return Task.FromResult(0);
                
            }, context);
            
            var sw = new Stopwatch();
            sw.Start();
            await _next.Invoke(context);
            sw.Stop();
            
            LogRequest(context, sw.ElapsedMilliseconds);
        }

        private void LogRequest(HttpContext context, long elapsed)
        {
            try
            {
                var request = new RequestLoggingModel
                {
                    Method = context.Request.Method,
                    ContentType = context.Request.ContentType,
                    Protocol = context.Request.Protocol,
                    Query = context.Request.QueryString.ToString(),
                    RequestURI = context.Request.Path,
                    Scheme = context.Request.Scheme,
                    UserAgent = context.Request.Headers["user-agent"],
                    ClientBuildId = context.Request.Headers["X-Client-BuildId"]
                };

                var response = new ResponseLoggingModel
                {
                    StatusCode = context.Response.StatusCode,
                    ReasonPhrase = ((HttpStatusCode) context.Response.StatusCode).ToString(),
                    Date = context.Response.Headers[HttpResponseHeader.Date.ToString()],
                    Server = context.Response.Headers[HttpResponseHeader.Server.ToString()],
                    ContentType = context.Response.ContentType
                };

                var sharedLoggingProperties = SharedLoggingProperties.GetAndDisposeSharedContextEntry(context.TraceIdentifier);
                sharedLoggingProperties.ElapsedMillis = elapsed;

                _logger.Information("{@Request} received and {@Response} generated in {ElapsedMillis} from {@CommunicationRespose}", request,
                    response, elapsed, sharedLoggingProperties);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not log request/response to elastic");
            }
        }
    }
}