namespace CM.Backend.Documents.GlobalLogging.Entities
{
    public class SharedContextProperties
    {
        public string MethodRouteTemplate { get; set; }
        public long ElapsedMillis { get; set; }
        
        public DbCommunicationInfo MongoDbCommunicationInfo { get; } = new DbCommunicationInfo();
        public DbCommunicationInfo PostgressCommunicationInfo { get; } = new DbCommunicationInfo();

        public void PrepareAsImmutableObject()
        {
            MongoDbCommunicationInfo.TruncateDbExecutionTimesToAverage();
            PostgressCommunicationInfo.TruncateDbExecutionTimesToAverage();
        }
    }
}