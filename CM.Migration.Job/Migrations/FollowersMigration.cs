using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Queries.Queries.MigrationSourceQueries;
using Marten.Util;
using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CM.Migration.Job.Migrations
{
    public class FollowersMigration
    {
        private readonly IMongoDatabase _source;
        private readonly FileHelper _fileHelper;
        private readonly ICommandRouter _commandRouter;
        private readonly IQueryRouter _queryRouter;

        private ErrorLogging<FollowerFollowing> _log;

        private Dictionary<string, Guid> _userIdConvertion;

        public FollowersMigration(IMongoDatabase source, FileHelper fileHelper, ICommandRouter commandRouter, IQueryRouter queryRouter)
        {
            _source = source;
            _fileHelper = fileHelper;
            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
            
            _userIdConvertion = new Dictionary<string, Guid>();
        }


        public async Task Execute()
        {
            _log = new ErrorLogging<FollowerFollowing>("Followers", "Followers migration");
            
            //Read the userIdConvertionLog
            Console.WriteLine("Do you want to continue the FollowerFollowing migration procedure? Press enter to proceed. Be aware that UserMigration.txt file must be present");
            //var input = Console.In.Read();
            
            var userMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/UserMigration.txt";
            
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
            
            var convertedFollowings = 0;
            
            Console.WriteLine("Pre-fetching migration resources...");
            
            var coll = _source.GetCollection<FollowerFollowing>("FollowerFollowing");
            var followings = coll.FindSync(FilterDefinition<FollowerFollowing>.Empty).ToList();
            
            Console.WriteLine("Migration resources fetched... Migrating Users...");

            foreach (var following in followings)
            {
                var followByIdConvertion = _userIdConvertion.Where(x => x.Key.Equals(following.Follower));
                var followToIdConvertion = _userIdConvertion.Where(x => x.Key.Equals(following.Following));

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
                
                
                var result = await _commandRouter.RouteAsync<FollowUser, Response>(
                    new FollowUser(followById, followToId));

                if (!result.IsSuccessful)
                {
                    Console.WriteLine("***** Failed to migrate following");
                    _log.WriteToErrorLog(following, $"Following:{following.Id} --> Failed to migrate ");
                }
                
                Console.WriteLine("Following Migrated");
                convertedFollowings++;
                Console.WriteLine("******************************");
                Console.WriteLine("Converted " + convertedFollowings + "/" + followings.Count);
                Console.WriteLine("******************************");
            }
            
            _log.FlushErrorLogToFile("");
        }


        public class FollowerFollowing : TEntity
        {
            [BsonId]
            public string Id { get; }
            
            [BsonElement(nameof(Following))]
            public string Following { get; set; }
            
            [BsonElement(nameof(Follower))]
            public string Follower { get; set; }
            
            [BsonElement(nameof(_created_at))]
            public DateTime _created_at { get; set; }
            
            [BsonElement(nameof(_updated_at))]
            public DateTime _updated_at { get; set; }
        }
    }
}