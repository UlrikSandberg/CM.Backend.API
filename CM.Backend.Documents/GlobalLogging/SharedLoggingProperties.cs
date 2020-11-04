using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using CM.Backend.Documents.GlobalLogging.Entities;

namespace CM.Backend.Documents.GlobalLogging
{   
    public static class SharedLoggingProperties
    {
        private static Dictionary<string, SharedContextProperties> SharedLoggingDictionary = new Dictionary<string, SharedContextProperties>();

        public static void CreateSharedContextEntry(string correlationId)
        {
            if (!SharedLoggingDictionary.ContainsKey(correlationId))
            {
                SharedLoggingDictionary.Add(correlationId, new SharedContextProperties()); 
            }
        }

        /// <summary>
        /// This will dispose the sharedContextEntry
        /// and prepare it as an immutable object
        /// with truncated data.
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public static SharedContextProperties GetAndDisposeSharedContextEntry(string correlationId)
        {
            if (SharedLoggingDictionary.ContainsKey(correlationId))
            {
                var entry = SharedLoggingDictionary[correlationId];
                
                DisposeSharedContextEntry(correlationId);
                
                entry.PrepareAsImmutableObject();

                return entry;
            }

            return null;
        }

        private static void DisposeSharedContextEntry(string correlationId)
        {
            if (SharedLoggingDictionary.ContainsKey(correlationId))
            {
                SharedLoggingDictionary.Remove(correlationId);
            }
        }

        public static void AddMethodRouteTemplate(string correlationId, string methodRouteTemplate)
        {
            CreateSharedContextEntry(correlationId);
            
            SharedLoggingDictionary[correlationId].MethodRouteTemplate = methodRouteTemplate;
        }

        public static void AddMongoDbCommunicationInfo(string correlationId, long elapsedTime, string cmdName)
        {
            CreateSharedContextEntry(correlationId);

            var sharedContext = SharedLoggingDictionary[correlationId];

            sharedContext.MongoDbCommunicationInfo.NumberOfDbCalls += 1;
            
            sharedContext.MongoDbCommunicationInfo.AddExecutionTime(cmdName, elapsedTime);

            sharedContext.MongoDbCommunicationInfo.TotalElapsedCommunicationTimeInMillis += elapsedTime;
        }

        public static void AddPostgressCommunicationInfo(string correlationId, long elapsedTime, string cmdName)
        {
            CreateSharedContextEntry(correlationId);

            var sharedContext = SharedLoggingDictionary[correlationId];

            sharedContext.PostgressCommunicationInfo.NumberOfDbCalls += 1;
            
            sharedContext.PostgressCommunicationInfo.AddExecutionTime(cmdName, elapsedTime);

            sharedContext.PostgressCommunicationInfo.TotalElapsedCommunicationTimeInMillis += elapsedTime;
        }
    }
}