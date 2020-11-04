using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using CM.Backend.API.Helpers;
using CM.Backend.Documents.StaticResources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace CM.Backend.API.ActionFilters
{
    /// <summary>
    /// 
    /// </summary>
    public class IPLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IOptions<IPStackConfiguration> _ipStackConfig;
        private readonly IMemoryCache _memoryCache;
        private const string NullIP = "::1";


        public IPLoggingMiddleware(RequestDelegate next, ILogger logger, IOptions<IPStackConfiguration> ipStackConfig,
            IMemoryCache memoryCache)
        {
            _next = next;
            _logger = logger;
            _ipStackConfig = ipStackConfig;
            _memoryCache = memoryCache;
        }

        private async Task<IPStackResponseModel> GetIPInfo(string remoteIp)
        {
            return await _memoryCache.GetOrCreateAsync(remoteIp, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromDays(1);
                
                _logger.Information("IP country unknown and not present in cache, performing external lookup...");
                
                var ipStackBaseUrl =
                    $"{_ipStackConfig.Value.BaseUrl}/{remoteIp}?access_key={_ipStackConfig.Value.API_Key}";

                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(ipStackBaseUrl);
                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadAsAsync<IPStackResponseModel>();

                    _logger.Error("Got error from IPStack: {HttpStatus}/{ErrorMessage}", response.StatusCode,
                        response.ReasonPhrase);
                    return null;
                }
            });
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                //Read cloudflare connection headers - https://support.cloudflare.com/hc/en-us/articles/200170986-How-does-Cloudflare-handle-HTTP-Request-headers-
                context.Request.Headers.TryGetValue("Cf-Connecting-IP", out var cfConnectingIp);
                context.Request.Headers.TryGetValue("Cf-Ipcountry", out var cfIpCountry);
                context.Request.Headers.TryGetValue("Cf-Ray", out var cfRay);

                var remoteIp = cfConnectingIp.FirstOrDefault() ?? context.Connection.RemoteIpAddress.ToString();

                LogContext.PushProperty("ClientIp", remoteIp);
                LogContext.PushProperty("CfIpCountry", cfIpCountry.FirstOrDefault());
                LogContext.PushProperty("CfRay", cfRay.FirstOrDefault());

                if (!remoteIp.Equals(NullIP))
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    var ipInfo = await GetIPInfo(remoteIp);

                    if (ipInfo != null)
                    {
                        var geoModel = GenericMapper<IPStackResponseModel, IPGeolocationModel>.Map(
                            new MapperConfiguration(
                                cfg =>
                                {
                                    cfg.CreateMap<IPStackResponseModel, IPGeolocationModel>()
                                        .ForMember(x => x.GeonameId,
                                            opt => opt.MapFrom(src => src.Location.Geoname_id));
                                }), ipInfo);

                        geoModel.SetLocationArr();
                        sw.Stop();

                        LogContext.PushProperty("Location", geoModel.Location);
                        _logger.Information("Request received from {IP} in {ElapsedMillis} ms, GeoDetails: {@GeoModel}, {Location}",
                            context.Connection.RemoteIpAddress.ToString(), sw.ElapsedMilliseconds, geoModel, geoModel.Location);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error looking up countrycode from IP from external service. Continuing processing request..");
                
                //Continue processing - this is not (that) critical
            }

            await _next.Invoke(context);
        }
    }
}