using CM.Backend.Eventstore.Persistence.Registry;
using CM.Instrumentation.Registry;
using SimpleSoft.Mediator;

namespace CM.Backend.Commands.Registry
{
    public class CommandRegistry : StructureMap.Registry
    {
        public CommandRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssemblyContainingType<CommandRegistry>(); // Our assembly with requests & handlers

                scanner.ConnectImplementationsToTypesClosing(typeof(ICommandHandler<>));
                scanner.ConnectImplementationsToTypesClosing(typeof(ICommandHandler<,>));
            });

            IncludeRegistry<EventstorePersistenceRegistry>();
            IncludeRegistry<InstrumentationRegistry>();
        }
    }
}