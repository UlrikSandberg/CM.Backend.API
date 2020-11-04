using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Baseline;
using CM.Backend.Commands.Commands.TastingCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.User;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Queries.Queries.MigrationSourceQueries;
using Marten.Util;
using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CM.Migration.Job.Migrations
{
    public class TastingMigration
    {
        private readonly IMongoDatabase _source;
        private readonly FileHelper _fileHelper;
        private readonly ICommandRouter _commandRouter;
        private readonly IQueryRouter _queryRouter;


        private Dictionary<string, Guid> _brandIdConversion;
        private Dictionary<string, Guid> _champagneIdConversion;
        private Dictionary<string, Guid> _userIdConversion;

        private ErrorLogging<Tasting> _log;
        
        public TastingMigration(IMongoDatabase source, FileHelper fileHelper, ICommandRouter commandRouter, IQueryRouter queryRouter)
        {
            _source = source;
            _fileHelper = fileHelper;
            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
            
            _brandIdConversion = new Dictionary<string, Guid>();
            _champagneIdConversion = new Dictionary<string, Guid>();
            _userIdConversion = new Dictionary<string, Guid>();
        }

        public async Task Execute()
        {
            _log = new ErrorLogging<Tasting>("Tasting", "Tasting migration");
            Console.WriteLine("Do you want to continue the Tasting migration procedure? Press enter to proceed. Be aware that ChampagneMigration.txt, UserMigration.txt and BrandMigration.txt files all much be present");
            //var input = Console.In.Read();
            
            //Read out data from previous migrations
            var brandMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/BrandMigration.txt";
            var champagneMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/ChampagneMigration.txt";
            var userMigrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/UserMigration.txt";
            
            //Map brand
            Console.WriteLine("Reading from brand conversion file");
            using (StreamReader sr = File.OpenText(brandMigrationPath)) 
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
                        _brandIdConversion.Add(oldId, newId);
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
                        _champagneIdConversion.Add(oldId, newId);
                    }
                }
            }
            
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
                        _userIdConversion.Add(oldId, newId);
                    }
                }
            }
            
            var convertedTastings = 0;
            
            Console.WriteLine("Pre-fetching migration resources...");
            
            var coll = _source.GetCollection<Tasting>("Post");
            var tastings = coll.FindSync(FilterDefinition<Tasting>.Empty).ToList();
            
            Console.WriteLine("Migration resources fetched... Migrating Users...");

            foreach (var tasting in tastings)
            {
                var migrationSource = await _queryRouter.QueryAsync<GetMigrationSourceBySourceId, MigrationSource>(
                    new GetMigrationSourceBySourceId(tasting.Id));

                if (migrationSource != null)
                {
                    Console.WriteLine($"Tasting:{tasting.Id} has already been migrated");
                    _log.WriteToConvertionLog(tasting.Id, migrationSource.Id,
                        $"Tasting migrated: old Id: {tasting.Id} --> new Id: {migrationSource.Id} --> $${tasting.Id} ; $${migrationSource.Id}");
                    convertedTastings++;
                    Console.WriteLine("******************************");
                    Console.WriteLine("Converted " + convertedTastings + "/" + tastings.Count);
                    Console.WriteLine("******************************");
                    continue;
                }
                
                //Resolve that the necessary information regarding brandId, champagneId, and userIds are present.
                var newBrandId = _brandIdConversion.Where(x => x.Key.Equals(tasting.BrandId));
                if (!newBrandId.Any())
                {
                    //This tasting is not connected to any brandId!
                    Console.WriteLine($"{tasting.Id} - for bottle:{tasting.BottleId} - by user:{tasting.userId} - could not be migrated due to invalid brandId:{tasting.BrandId}");
                    _log.WriteToErrorLog(tasting, $"{tasting.Id} - for bottle:{tasting.BottleId} - by user:{tasting.userId} - could not be migrated due to invalid brandId:{tasting.BrandId}");
                    continue;
                }
                var brandId = newBrandId.First().Value; //<-- Assign value of key.

                var newChampagneId = _champagneIdConversion.Where(x => x.Key.Equals(tasting.BottleId));
                if (!newChampagneId.Any())
                {
                    //This tasting is not connected to any known champagnId
                    Console.WriteLine($"{tasting.Id} - for bottle:{tasting.BottleId} - by user:{tasting.userId} - could not be migrated due to invalid champagneId:{tasting.BottleId}");
                    _log.WriteToErrorLog(tasting, $"{tasting.Id} - for bottle:{tasting.BottleId} - by user:{tasting.userId} - could not be migrated due to invalid champagneId:{tasting.BottleId}");
                    continue;
                }
                var champagneId = newChampagneId.First().Value;

                var newUserId = _userIdConversion.Where(x => x.Key.Equals(tasting.userId));
                if (!newUserId.Any())
                {
                    //This tasting is not connected to any known userId
                    Console.WriteLine($"{tasting.Id} - for bottle:{tasting.BottleId} - by user:{tasting.userId} - could not be migrated due to invalid userId:{tasting.userId}");
                    _log.WriteToErrorLog(tasting, $"{tasting.Id} - for bottle:{tasting.BottleId} - by user:{tasting.userId} - could not be migrated due to invalid userId:{tasting.userId}");
                    continue;
                }
                var userId = newUserId.First().Value;
                Console.WriteLine("Migrating tasting 0%");
                //Ready to migrate tasting
                var newId = await _commandRouter.RouteAsync<MigrateTasting, IdResponse>(
                    new MigrateTasting(
                        new CreateTasting(
                        userId,
                        champagneId,
                        tasting.Review,
                        tasting.Rating,
                        tasting._created_at), MigrationSourceEnum.OldLive.ToString(), tasting.Id));

                if (newId.IsSuccessful)
                {
                    Console.WriteLine($"Tasting migrated: old Id: {tasting.Id} --> new Id: {newId.Id} --> $${tasting.Id} ; $${newId.Id}");
                    _log.WriteToConvertionLog(tasting.Id, newId.Id, $"Tasting migrated: old Id: {tasting.Id} --> new Id: {newId.Id} --> $${tasting.Id} ; $${newId.Id}");
                    convertedTastings++;
                    Console.WriteLine("******************************");
                    Console.WriteLine("Converted " + convertedTastings + "/" + tastings.Count);
                    Console.WriteLine("******************************");
                }
                else
                {
                    _log.WriteToErrorLog(tasting, $"****** Error migrating tasting ***** {tasting.Id} - for user:{tasting.userId}");
                    Console.WriteLine($"****** Error migrating tasting ***** {tasting.Id} - for user:{tasting.userId}");
                }
            }
            _log.FlushErrorLogToFile("");
        }

        public class Tasting : TEntity
        {
            [BsonId]
            public string Id { get; set; }
            
            [BsonElement(nameof(AromaScore))]
            public double AromaScore { get; set; }
            
            [BsonElement(nameof(BrandName))]
            public string BrandName { get; set; }
            
            [BsonElement(nameof(Visual))]
            public string Visual { get; set; }
            
            [BsonElement(nameof(Taste))]
            public string Taste { get; set; }
            
            [BsonElement(nameof(ReviewEmpty))]
            public bool ReviewEmpty { get; set; }
            
            [BsonElement(nameof(Relations))]
            public string[] Relations { get; set; }
            
            [BsonElement(nameof(food))]
            public string food { get; set; }
            
            [BsonElement(nameof(uuId))]
            public Guid uuId { get; set; }
            
            [BsonElement(nameof(VisualScore))]
            public double VisualScore { get; set; }
            
            [BsonElement(nameof(userId))]
            public string userId { get; set; }
            
            [BsonElement(nameof(BottleId))]
            public string BottleId { get; set; }
            
            [BsonElement(nameof(BrandId))]
            public string BrandId { get; set; }
            
            [BsonElement(nameof(Aroma))]
            public string Aroma { get; set; }
            
            [BsonElement(nameof(CurrencyNumber))]
            public double CurrencyNumber { get; set; }
            
            [BsonElement(nameof(Rating))]
            public double Rating { get; set; }
            
            [BsonElement(nameof(MasterId))]
            public string MasterId { get; set; }
            
            [BsonElement(nameof(TasteScore))]
            public double TasteScore { get; set; }
            
            [BsonElement(nameof(Review))]
            public string Review { get; set; }
            
            [BsonElement(nameof(Price))]
            public double Price { get; set; }
            
            [BsonElement(nameof(_created_at))]
            public DateTime _created_at { get; set; }
            
            [BsonElement(nameof(_updated_at))]
            public DateTime _updated_at { get; set; } 
        }
    }
}