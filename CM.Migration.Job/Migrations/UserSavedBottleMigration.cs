using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CM.Backend.API.EnumOptions;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CM.Migration.Job.Migrations
{
    public class UserSavedBottleMigration
    {
        private readonly IMongoDatabase _source;
        private readonly FileHelper _fileHelper;
        private readonly ICommandRouter _commandRouter;
        private readonly IQueryRouter _queryRouter;

        private ErrorLogging<UserSavedBottles> _log;

        private Dictionary<string, Guid> _userIdConvertion;
        private Dictionary<string, Guid> _champagneIdConvertion;

        public UserSavedBottleMigration(IMongoDatabase source, FileHelper fileHelper, ICommandRouter commandRouter, IQueryRouter queryRouter)
        {
            _source = source;
            _fileHelper = fileHelper;
            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
            
            _userIdConvertion = new Dictionary<string, Guid>();
            _champagneIdConvertion = new Dictionary<string, Guid>();
        }


        public async Task Execute()
        {
            _log = new ErrorLogging<UserSavedBottles>("UserSavedBottles", "UserSavedBottles migration");
            
            //Read the userIdConvertionLog
            Console.WriteLine("Do you want to continue the UserSavedBottles migration procedure? Press enter to proceed. Be aware that UserMigration.txt and champagneMigration.txt files must be present");
            //var input = Console.In.Read();
            
            var userMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/UserMigration.txt";
            var champagneMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/ChampagneMigration.txt";

            
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
            
            Console.WriteLine("Reading from champagne conversion file");
            using (StreamReader sr = File.OpenText(champagneMigrationPath)) 
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
                        _champagneIdConvertion.Add(oldId, newId);
                    }
                }
            }
            
            var convertedSavedBottles = 0;
            
            Console.WriteLine("Pre-fetching migration resources...");
            
            var coll = _source.GetCollection<UserSavedBottles>("userSavedBottles");
            var savedBottles = coll.FindSync(FilterDefinition<UserSavedBottles>.Empty).ToList();
            
            Console.WriteLine("Migration resources fetched... Migrating Users...");

            foreach (var savedBottle in savedBottles)
            {
                var userIdConvertion = _userIdConvertion.Where(x => x.Key.Equals(savedBottle.userId));
                var bottleIdConvertion = _champagneIdConvertion.Where(x => x.Key.Equals(savedBottle.bottleId));

                if (!userIdConvertion.Any())
                {
                    Console.WriteLine($"savedBottle:{savedBottle.Id} --> Could not be migrated");
                    _log.WriteToErrorLog(savedBottle, $"Following:{savedBottle.Id} could not be migrated userId was not found in userIdConvertionTable");
                    continue;
                }

                var userId = userIdConvertion.First().Value;

                if (!bottleIdConvertion.Any())
                {
                    Console.WriteLine($"savedBottle:{savedBottle.Id} --> Could not be migrated");
                    _log.WriteToErrorLog(savedBottle, $"Following:{savedBottle.Id} could not be migrated bottleId was not found in champagneIdConvertionTable");
                    continue;
                }

                var bottleId = bottleIdConvertion.First().Value;

                var result =
                    await _commandRouter.RouteAsync<BookmarkChampagne, Response>(
                        new BookmarkChampagne(userId, bottleId));
                
                if (!result.IsSuccessful)
                {
                    Console.WriteLine("***** Failed to migrate savedBottle");
                    _log.WriteToErrorLog(savedBottle, $"savedBottle:{savedBottle.Id} --> Failed to migrate ");
                }
                
                Console.WriteLine("savedBottle migrated");
                convertedSavedBottles++;
                Console.WriteLine("******************************");
                Console.WriteLine("Converted " + convertedSavedBottles + "/" + savedBottles.Count);
                Console.WriteLine("******************************");
            }
            
            _log.FlushErrorLogToFile("");
        }
    }

    public class UserSavedBottles : TEntity
    {
        [BsonId]
        public string Id { get; set; }
        
        [BsonElement(nameof(userId))]
        public string userId { get; set; }
        
        [BsonElement(nameof(bottleId))]
        public string bottleId { get; set; }
        
        [BsonElement(nameof(_created_at))]
        public DateTime _created_at { get; set; }
        
        [BsonElement(nameof(_updated_at))]
        public DateTime _updated_at { get; set; }
    }
}