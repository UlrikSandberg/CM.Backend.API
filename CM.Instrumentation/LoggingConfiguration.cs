using System;
using Elasticsearch.Net;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace CM.Instrumentation
{
    public class LoggingConfiguration
    {
        public static Serilog.LoggerConfiguration GetConfiguration(InstrumentationConfiguration config)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.WithEnvironment("Environment")
                .Enrich.FromLogContext()
                .Enrich.WithThreadId()
                //.WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(config.ElasticsearchUrl))
                    {
                        AutoRegisterTemplate = true,
                        ModifyConnectionSettings = x => x.BasicAuthentication(config.Username, config.Password)
                    })
                .WriteTo.ApplicationInsightsEvents(TelemetryConfiguration.Active);
        }
    }
}