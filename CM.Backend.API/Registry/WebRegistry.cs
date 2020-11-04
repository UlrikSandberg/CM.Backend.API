using CM.Backend.API.ActionFilters;
using CM.Backend.API.Helpers;
using CM.Backend.Messaging.Infrastructure.Registry;
using CM.Instrumentation.Registry;

namespace CM.Backend.API.Registry
{
    public class WebRegistry : StructureMap.Registry
    {
        public WebRegistry()
        {   
            IncludeRegistry<InstrumentationRegistry>();
            IncludeRegistry<MessagingInfrastructureRegistry>();
            For<IImageResizer>().Use<ImageResizer>();
            
            ForConcreteType<ClientValidationFilter>();
        }
    }
}
