using CM.Backend.Commands.Registry;
using CM.Backend.EventHandlers.Registry;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Messaging.Infrastructure.Factories;
using CM.Backend.Queries.Registry;
using CM.Instrumentation.Registry;
using SimpleSoft.Mediator;

namespace CM.Backend.Messaging.Infrastructure.Registry
{
    public class MessagingInfrastructureRegistry : StructureMap.Registry
    {
        public MessagingInfrastructureRegistry()
        {
            For<IMediator>().Use<Mediator>();
            For<IMediatorFactory>().Use<StructureMapMediatorFactory>();

            For<IEventPublisher>().Use<MediatorEventPublisher>();
            For<ICommandRouter>().Use<MediatorCommandRouter>();
            For<IQueryRouter>().Use<MediatorQueryRouter>();

            IncludeRegistry<CommandRegistry>();
            IncludeRegistry<EventHandlerRegistry>();
            IncludeRegistry<QueryRegistry>();
            IncludeRegistry<InstrumentationRegistry>();
        }

    }
}