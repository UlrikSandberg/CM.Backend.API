using Microsoft.Extensions.Options;
using Serilog;

namespace CM.Instrumentation.Registry
{
    public class InstrumentationRegistry : StructureMap.Registry
    {
        public InstrumentationRegistry()
        {
            For<ILogger>().Use(ctx => LoggingConfiguration.GetConfiguration(ctx.GetInstance<IOptions<InstrumentationConfiguration>>().Value).CreateLogger());
        }
    }
}