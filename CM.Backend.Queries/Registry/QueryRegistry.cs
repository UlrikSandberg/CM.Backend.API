using CM.Backend.Persistence.Registries;
using SimpleSoft.Mediator;

namespace CM.Backend.Queries.Registry
{
    public class QueryRegistry : StructureMap.Registry
    {
        public QueryRegistry()
        {
            Scan(scanner =>
            {
                scanner.AssemblyContainingType<QueryRegistry>(); // Our assembly with requests & handlers
                scanner.ConnectImplementationsToTypesClosing(typeof(IQueryHandler<,>));
            });

            IncludeRegistry<PersistenceRegistry>();
        }
    }
}