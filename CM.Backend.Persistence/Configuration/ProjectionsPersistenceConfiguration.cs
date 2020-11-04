namespace CM.Backend.Persistence.Configuration
{
    public class ProjectionsPersistenceConfiguration
    {
        public string MongoClusterConnectionString { get; set; }

        public string DefaultProjectionsDatabaseName { get; set; }

        public string AzureStorageAccountConnectionString { get; set; }

    }
}