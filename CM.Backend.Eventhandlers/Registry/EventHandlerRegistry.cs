using CM.Backend.Persistence.Registries;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers.Registry
{
    public class EventHandlerRegistry : StructureMap.Registry
    {
        public EventHandlerRegistry()
        {
            Scan(x =>
            {
                x.AssemblyContainingType<EventHandlerRegistry>();
                x.ConnectImplementationsToTypesClosing(typeof(IEventHandler<>));
            });

            IncludeRegistry<PersistenceRegistry>();
        }
    }
}