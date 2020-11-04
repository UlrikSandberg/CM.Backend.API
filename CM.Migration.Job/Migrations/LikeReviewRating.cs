using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CM.Backend.Messaging.Contracts;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CM.Migration.Job.Migrations
{
    public class LikeReviewRatingMigration
    {
        private readonly IMongoDatabase _source;
        private readonly FileHelper _fileHelper;
        private readonly ICommandRouter _commandRouter;
        private readonly IQueryRouter _queryRouter;

        private ErrorLogging<LikeReview> _log;

        private Dictionary<string, Guid> _userIdConvertion;
        private Dictionary<string, Guid> _tastingIdConvertion;

        public LikeReviewRatingMigration(IMongoDatabase source, FileHelper fileHelper, ICommandRouter commandRouter, IQueryRouter queryRouter)
        {
            _source = source;
            _fileHelper = fileHelper;
            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
            
            _userIdConvertion = new Dictionary<string, Guid>();
            _tastingIdConvertion = new Dictionary<string, Guid>();
        }


        public async Task Execute()
        {
            //_log = new ErrorLogging<LikeReview>("LikeReviewRating", "LikeReviewRating migration");
            
            /*//Read the userIdConvertionLog
            Console.WriteLine("Do you want to continue the LikeReview migration procedure? Press enter to proceed. Be aware that UserMigration.txt and TastingMigration.txt files must be present");
            var input = Console.In.Read();
            
            var userMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/UserMigration.txt";
            var tastingMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/TastingMigration.txt";
            
            Console.WriteLine("Reading from user conversion file");
            using (StreamReader sr = File.OpenText(userMigrationPath)) 
            {
                string s = "";
                while ((s = sr.ReadLine()) != null) 
                {
                    //Split s into tokens;
                    var split = s.Split("$$");
                    if (split.Length == 3)
                    {
                        var oldId = split[1].Replace(" ; ", "");
                        var newId = Guid.Parse(split[2]);
                        _userIdConvertion.Add(oldId, newId);
                    }
                }
            }
            
            Console.WriteLine("Reading from tasting conversion file");
            using (StreamReader sr = File.OpenText(tastingMigrationPath)) 
            {
                string s = "";
                while ((s = sr.ReadLine()) != null) 
                {
                    //Split s into tokens;
                    var split = s.Split("$$");
                    if (split.Length == 3)
                    {
                        var oldId = split[1].Replace(" ; ", "");
                        var newId = Guid.Parse(split[2]);
                        _tastingIdConvertion.Add(oldId, newId);
                    }
                }
            }
            
            var convertedLikes = 0;
            
            Console.WriteLine("Pre-fetching migration resources...");
            
            var coll = _source.GetCollection<LikeReview>("LikeReviewRating");
            var likes = coll.FindSync(FilterDefinition<LikeReview>.Empty).ToList();
            
            Console.WriteLine("Migration resources fetched... Migrating Users...");

            foreach (var like in likes)
            {
                var likeByIdConvertion = _userIdConvertion.Where(x => x.Key.Equals(like.));
                var likeToContextIdConvertion = _brandIdConvertion.Where(x => x.Key.Equals(following.FollowTo));

                if (!followByIdConvertion.Any())
                {
                    Console.WriteLine($"Following:{following.Id} --> Could not be migrated");
                    _log.WriteToErrorLog(following, $"Following:{following.Id} could not be migrated followById was not found in userIdConvertionTable");
                    continue;
                }

                var followById = followByIdConvertion.First().Value;

                if (!followToIdConvertion.Any())
                {
                    Console.WriteLine($"Following:{following.Id} --> Could not be migrated");
                    _log.WriteToErrorLog(following, $"Following:{following.Id} could not be migrated followToId was not found in userIdConvertionTable");
                    continue;
                }

                var followToId = followToIdConvertion.First().Value;

                
                if (!result.IsSuccessful)
                {
                    Console.WriteLine("***** Failed to migrate brandfollowers");
                    _log.WriteToErrorLog(following, $"Following:{following.Id} --> Failed to migrate ");
                }
                
                Console.WriteLine("BrandFollowers Migrated");
                convertedFollowings++;
                Console.WriteLine("******************************");
                Console.WriteLine("Converted " + convertedFollowings + "/" + followings.Count);
                Console.WriteLine("******************************");
            }
            
            //_log.FlushErrorLogToFile("");*/
        }


        public class LikeReview : TEntity
        {
            [BsonId]
            public string Id { get; }
            
            [BsonElement(nameof(FollowTo))]
            public string FollowTo { get; set; }
            
            [BsonElement(nameof(FollowBy))]
            public string FollowBy { get; set; }
            
            [BsonElement(nameof(_created_at))]
            public DateTime _created_at { get; set; }
            
            [BsonElement(nameof(_updated_at))]
            public DateTime _updated_at { get; set; }
        }
    }
}