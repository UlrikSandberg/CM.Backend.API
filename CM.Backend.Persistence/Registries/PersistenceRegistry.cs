using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Repositories;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CM.Backend.Persistence.Registries
{
    public class PersistenceRegistry : StructureMap.Registry
    {
        public PersistenceRegistry()
        {
            var databasename = "cm-staging";
            For<IMongoClient>().Use(ctx => new MongoClient(ctx.GetInstance<IOptions<ProjectionsPersistenceConfiguration>>().Value.MongoClusterConnectionString)).Singleton();

            For<IMongoDatabase>().Use(ctx => ctx.GetInstance<IMongoClient>().GetDatabase(databasename, null));

            Scan(cfg =>
            {
                cfg.AssemblyContainingType<PersistenceRegistry>();
                cfg.SingleImplementationsOfInterface();
            });

            For<IChampagneRepository>().Use<ChampagneRepository>();
        }
    }
}