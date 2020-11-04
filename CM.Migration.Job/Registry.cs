using System;
using System.IO;
using CM.Backend.API.Helpers;
using CM.Backend.EventHandlers.Configuration;
using CM.Backend.Eventstore.Persistence.Configuration;
using CM.Backend.Messaging.Infrastructure.Registry;
using CM.Backend.Persistence.Configuration;
using CM.Instrumentation;
using CM.Instrumentation.Registry;
using CM.Migration.Job.Migrations;
using IdentityModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CM.Migration.Job
{
    public class Registry : StructureMap.Registry
    {
        public Registry()
        {            
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
                throw new ArgumentNullException("Environment not found in ASPNETCORE_ENVIRONMENT - where do you want to run this?");

            Console.WriteLine("Environment: {0}", environment);

            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: false);

            var config = builder.Build();
            
            IncludeRegistry<InstrumentationRegistry>();
            IncludeRegistry<MessagingInfrastructureRegistry>();

            ForConcreteType<BrandMigration>();
            ForConcreteType<ChampagneMigration>();
            ForConcreteType<UserMigration>();
            ForConcreteType<TastingMigration>();
            ForConcreteType<FollowersMigration>();
            ForConcreteType<BrandFollowersMigration>();
            ForConcreteType<UserSavedBottleMigration>();
            
            var client = new MongoClient("mongodb://cm-liveuser:dlk11j5u17@ds163324-a0.mlab.com:63324,ds163324-a1.mlab.com:63324/cm-live?replicaSet=rs-ds163324");

            For<IMongoDatabase>().Use(client.GetDatabase("cm-live"));
            
            For<IOptions<EventstoreConfiguration>>().Use(Options.Create(config.GetSection(nameof(EventstoreConfiguration)).Get<EventstoreConfiguration>()));
            For<IOptions<InstrumentationConfiguration>>().Use(Options.Create(config.GetSection(nameof(InstrumentationConfiguration)).Get<InstrumentationConfiguration>()));
            For<IOptions<ProjectionsPersistenceConfiguration>>().Use(Options.Create(config.GetSection(nameof(ProjectionsPersistenceConfiguration)).Get<ProjectionsPersistenceConfiguration>()));
            For<IOptions<InstrumentationConfiguration>>().Use(Options.Create(config.GetSection(nameof(InstrumentationConfiguration)).Get<InstrumentationConfiguration>()));
            For<IOptions<IdentityServerConfiguration>>().Use(Options.Create(config
                .GetSection(nameof(IdentityServerConfiguration)).Get<IdentityServerConfiguration>()));
            For<IOptions<EventHandlerCMBackendUrlConfiguration>>().Use(Options.Create(config.GetSection("CMBackendUrlConfiguration").Get<EventHandlerCMBackendUrlConfiguration>()));
            For<IOptions<EventHandlerSendGridConfiguration>>().Use(
                Options.Create(config.GetSection("SendGridConfiguration").Get<EventHandlerSendGridConfiguration>()));
            For<IOptions<EmailAuthorityConfiguration>>().Use(Options.Create(config
                .GetSection(nameof(EmailAuthorityConfiguration)).Get<EmailAuthorityConfiguration>()));
            For<IOptions<NotificationHubConfiguration>>().Use(Options.Create(config
                .GetSection(nameof(NotificationHubConfiguration)).Get<NotificationHubConfiguration>()));
        }
    }
}
