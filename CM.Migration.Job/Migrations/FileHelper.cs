using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using SimpleSoft.Mediator;

namespace CM.Migration.Job.Migrations
{
    public class FileHelper
    {
        private readonly IMongoDatabase _database;

        public FileHelper(IMongoDatabase database)
        {
            _database = database;
        }
        
        public async Task<byte[]> DownloadFile(string fileName)
        {
            var bucket = new GridFSBucket(_database);
            return await bucket.DownloadAsBytesByNameAsync(fileName);
        }
        
        public async Task<Stream> DownloadFileAsStream(string fileName)
        {
            var bucket = new GridFSBucket(_database);
            return await bucket.OpenDownloadStreamByNameAsync(fileName);
        }
    }
}