using System;
using CM.Backend.Eventstore.Persistence.Configuration;
using Marten;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using StructureMap;

namespace CM.Backend.Eventstore.Persistence.Registry
{
    public class EventstorePersistenceRegistry : StructureMap.Registry
    {
        public EventstorePersistenceRegistry()
        {
            var pack = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };
            ConventionRegistry.Register("camelCase", pack, t => true);
            BsonSerializer.RegisterSerializer(typeof(DateTime), DateTimeSerializer.UtcInstance);
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            For<IDocumentStore>().Use(ctx => ConfigureEventstore(ctx));
            For<IPublishingAggregateRepository>().Use<PublishingAggregateRepository>();
        }

        private IDocumentStore ConfigureEventstore(IContext ctx)
        {
            return DocumentStore.For(opt =>
            {
                var config = ctx.GetInstance<IOptions<EventstoreConfiguration>>().Value;
                
                opt.Connection(config.ConnectionString);

                opt.CreateDatabases = db => db.ForTenant("champagne");
                opt.AutoCreateSchemaObjects = AutoCreate.All;
                opt.DatabaseSchemaName = config.Schema;
            });
        }
    }
}