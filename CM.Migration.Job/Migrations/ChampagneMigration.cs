using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.API.Controllers;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.RequestModels;
using CM.Backend.Commands.Commands;
using CM.Backend.Commands.Commands.BrandCommands;
using CM.Backend.Commands.Handlers;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Champagne.Events;
using CM.Backend.Domain.Aggregates.Champagne.ValueObjects;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Queries.Model;
using CM.Backend.Queries.Queries;
using CM.Backend.Queries.Queries.MigrationSourceQueries;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimpleSoft.Mediator;
using StructureMap;
using StructureMap.Diagnostics;

namespace CM.Migration.Job.Migrations
{
    public class ChampagneMigration
    {
        //Old brandId --> New Guid.Id
        private Dictionary<string, Guid> _brandIdConvertion;
        //Old masterId --> New edition champagneFolder.Id
        private Dictionary<string, Guid> _masterIdToEditionFolderConvertion;

        public Dictionary<string, Guid> _champagneIdConvertion;

        private Dictionary<Champagne, List<string>> _errorLog;
        
        private readonly IMongoDatabase _source;
        private readonly BrandsController _brandsController;
        private readonly FileHelper _fileHelper;
        private readonly ICommandRouter _commandRouter;
        private readonly IQueryRouter _queryRouter;

        public ChampagneMigration(IMongoDatabase source, BrandsController brandsController, FileHelper fileHelper, ICommandRouter commandRouter, IQueryRouter queryRouter)
        {
            _source = source;
            _brandsController = brandsController;
            _fileHelper = fileHelper;
            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
            _brandIdConvertion = new Dictionary<string, Guid>();
            _masterIdToEditionFolderConvertion = new Dictionary<string, Guid>();
            _errorLog = new Dictionary<Champagne, List<string>>();
            _champagneIdConvertion = new Dictionary<string, Guid>();
        }

        public async Task Execute()
        {
           
            
            Console.WriteLine("Do you want to continue the ChampagneMigration procedure? Press enter to proceed. Be aware that BrandMigration.txt must be present for success");
            //var input = Console.In.Read();
            
            //Fetch file
            string txtPath = Directory.GetCurrentDirectory() + "/MigrationLogs/BrandMigration.txt";
            if (!File.Exists(txtPath))
            {
                throw new Exception("No brand migration file at: " + txtPath);
            }
            
            using (StreamReader sr = File.OpenText(txtPath)) 
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
                        _brandIdConvertion.Add(oldId, newId);
                    }
                }
            }
            
            Console.WriteLine("Pre-fetching migration resources...");
            var coll = _source.GetCollection<ChampagneMigration.Champagne>("ChampagneProfile");
            var champagnes = coll.FindSync(FilterDefinition<ChampagneMigration.Champagne>.Empty).ToList();

            //Get CPData and create an extended champagne with all the data required... Namely the extra attributes such as style, character and dosage from CPData.
            var coll1 = _source.GetCollection<ChampagneMigration.CPData>("CPData");
            var cpDatas = coll1.FindSync(FilterDefinition<ChampagneMigration.CPData>.Empty).ToList();

            var convertedCount = 0;
            foreach (var cpData in cpDatas)
            {
                var champagne = champagnes.Where(x => x.Id == cpData.CPId);
                if (champagne.Any())
                {
                    champagne.First().Style = cpData.style;
                    champagne.First().Character = cpData.Character;
                    convertedCount++;
                }
            }
            
            //Merge all data from coll and coll1.
            var convertedChampagnes = 0;
            
            //Create a log file!
            var migrationPath = CreateChampagneMigrationLog();
            
            ValidateConversion(_brandIdConvertion, champagnes);
            
            foreach(var champagne in champagnes)
            {
                //check that the champagne has not already been migrated...
                var result = await _queryRouter.QueryAsync<GetMigrationSourceBySourceId, MigrationSource>(
                    new GetMigrationSourceBySourceId(champagne.Id));

                if (result != null)
                {
                    Console.WriteLine($"{champagne.BottleName} already migrated --> Continuing...");
                    convertedChampagnes++;
                    Console.WriteLine("******************************");
                    Console.WriteLine("Converted " + convertedChampagnes + "/" + champagnes.Count);
                    Console.WriteLine("******************************");
                    
                    using (StreamWriter sw = File.AppendText(migrationPath)) 
                    {
                        _champagneIdConvertion.Add(champagne.Id, result.Id);
                        sw.WriteLine($"Champagne migrated: {champagne.BottleName} old Id: {champagne.Id} --> new Id: {result.Id} --> $${champagne.Id} ; $${result.Id}");
                    }
                    continue;
                }
                
                Console.WriteLine("Starting migration of champagne: " + champagne.BottleName);
                
                //Fetch brandId for this bottle
                var brandId = _brandIdConvertion[champagne.BrandId];

                int year = 0;
                var parseSucces = int.TryParse(champagne.VintageYear, out year);
                if (!parseSucces)
                {
                    var errString = "Failed to convert." + champagne.BottleName + " VintageYear: " +
                                    champagne.VintageYear + " could not be converted substituted with 0";
                    Console.WriteLine(errString);
                    WriteToErrorLog(champagne, errString);
                }
                
                //Upload image and retreive id! <-- Upload image
                Console.WriteLine("Uploading Image 0%");
                
                byte[] champagneFile = null;
                ObjectResult imageResult = null;
                Guid imageId = Guid.Empty;
                try
                {
                    champagneFile = await _fileHelper.DownloadFile(champagne.BottleImg);
                    imageResult = await _brandsController.UploadImages(
                        new FormFile(new MemoryStream(champagneFile), 0, champagneFile.Length, champagne.BottleName, champagne.BottleImg),
                        brandId, $"Champagne: {champagne.BottleName} - Year: {champagne.VintageYear}{Path.GetExtension(champagne.BottleImg)}", BrandTypes.TypeOfBrandImage.Champagne, false) as ObjectResult;
                    imageId = Guid.Empty;
                    var imageIdParseResult = Guid.TryParse(imageResult.Value.ToString(), out imageId);
                    if (!imageIdParseResult)
                    {
                        WriteToErrorLog(champagne, "Migration image failed --> ImageId set to Guid.Empty");
                        Console.WriteLine("Uploading Image failed --> Continuing migration.. Progress");
                    }
                    else
                    {
                        Console.WriteLine("Uploading Image 100% --> Continuing migration... Progress");
                    }
                }
                catch(Exception ex)
                {
                    WriteToErrorLog(champagne, "Migration image failed --> ImageId set to Guid.Empty" + ex.Message);
                    Console.WriteLine("Uploading Image failed --> Continuing migration.. Progress");
                }
                
                //Figure displayDosage --> DosageCodes,
                var dosage = ResolveDosage(champagne, champagne.Dosage);
                var dosageCodes = new HashSet<string>{dosage};
                
                //Figure displayStyle --> StyleCodes,
                var style = ResolveStyle(champagne, champagne.Style);
                var styleCodes = new HashSet<string>{style};
                //Figure displayCharacter --> characterCodes
                var character = ResolveCharacter(champagne, champagne.Character);
                var characterCodes = new HashSet<string>{character};
                
                var champagneProfile = new AddChampagneProfile
                (brandId,
                    Guid.Empty,
                    champagne.Appearance,
                    champagne.BlendInfo,
                    champagne.Taste,
                    "",
                    champagne.Aroma,
                    "",
                    dosage,
                    dosageCodes,
                    style,
                    styleCodes,
                    character,
                    characterCodes,
                    champagne.RedWineAmount,
                    0.0,
                    0.0,
                    0.0,
                    0.0,
                    champagne.AlcoholVol,
                    champagne.PinotNoir,
                    champagne.PinotMeunier,
                    champagne.Chardonnay
                );

                var publishCmd = new SetChampagnePublishingStatus(brandId, Guid.Empty, true);
                
                IdResponse newId = null;
                
                if (_masterIdToEditionFolderConvertion.Any(x => x.Key.Equals(champagne.MasterId)))
                {
                    var editionFolderId = _masterIdToEditionFolderConvertion
                        .FirstOrDefault(x => x.Key.Equals(champagne.MasterId)).Value;
                    //A masterId similar to this masterId has already been converted add to the same edition folder
                    newId = await _commandRouter.RouteAsync<MigrateChampagne, IdResponse>(new MigrateChampagne(
                        "OldLive",
                        champagne.Id,
                        new CreateChampagne(
                            champagne.BottleName,
                            brandId,
                            imageId,
                            champagne.Vintage,
                            year,
                            CreateChampagneWithOptions.AddToExistingEditionsFolder.ToString(),
                            editionFolderId,
                            false),
                        champagneProfile,
                        publishCmd));
                    Console.WriteLine("Champagne Migrated and added to champagneEditionFolder with id: " + editionFolderId);

                }
                else
                {
                    //A masterId similar to this masterId has already been converted add to the same edition folder
                    newId = await _commandRouter.RouteAsync<MigrateChampagne, IdResponse>(new MigrateChampagne(
                        MigrationSourceEnum.OldLive.ToString(),
                        champagne.Id,
                        new CreateChampagne(
                            champagne.BottleName,
                            brandId,
                            imageId,
                            champagne.Vintage,
                            year,
                            CreateChampagneWithOptions.CreateNewEditionFolderAndAddAfterCreation.ToString(),
                            Guid.Empty,
                            false),
                        champagneProfile,
                        publishCmd));
                    Console.WriteLine("Champagne Migrated with new editionFolder");
                    
                    if (newId.IsSuccessful)
                    {
                        //Update _masterIdToEditionFolderConvertion --> Fetch the newly created champagne
                        var query = await _queryRouter.QueryAsync<GetChampagne, ChampagneQueryModel>(
                            new GetChampagne(newId.Id, Guid.Empty));

                        _masterIdToEditionFolderConvertion.Add(champagne.MasterId, query.ChampagneRootId);
                    }
                }
                if (!newId.IsSuccessful)
                {
                    WriteToErrorLog(champagne, $"****** ERROR MIGRATING Champagne: {champagne.Id} : {champagne.BottleName}");
                    Console.Out.WriteLine($"****** ERROR MIGRATING Champagne: {champagne.Id} : {champagne.BottleName}");
                    continue;
                }
                
                using (StreamWriter sw = File.AppendText(migrationPath)) 
                {
                    _champagneIdConvertion.Add(champagne.Id, newId.Id);
                    sw.WriteLine($"Champagne migrated: {champagne.BottleName} old Id: {champagne.Id} --> new Id: {newId.Id} --> $${champagne.Id} ; $${newId.Id}");
                }
                
                convertedChampagnes++;
                Console.WriteLine("******************************");
                Console.WriteLine("Converted " + convertedChampagnes + "/" + champagnes.Count);
                Console.WriteLine("******************************");
                
            }
            
            FlushErrorLogToFile();
        }

        private void ValidateConversion(Dictionary<string, Guid> brandIdConversion, List<Champagne> toBeMigrated)
        {
            foreach (var champagne in toBeMigrated)
            {
                var brandId = brandIdConversion.Where(x => x.Key.Equals(champagne.BrandId));
                if (!brandId.Any())
                {
                    throw new Exception(
                        "Not all champagnes can be converted --> Some champagnes are missing a brandId conversion");
                }
            }
        }
        
        //Migration classes
        public class Champagne : TEntity
        {
            [BsonId]
            public string Id { get; set; }
            [BsonElement(nameof(BottleName))]
            public string BottleName { get; set; }
            [BsonElement(nameof(BrandNameDatabase))]
            public string BrandNameDatabase { get; set; }
            [BsonElement(nameof(BrandId))]
            public string BrandId { get; set; }
            [BsonElement(nameof(Mother))]
            public string Mother { get; set; }
            [BsonElement(nameof(isMother))]
            public bool isMother { get; set; }
            [BsonElement(nameof(MasterId))]
            public string MasterId { get; set; }
            [BsonElement(nameof(BottleImg))]
            public string BottleImg { get; set; }
            
            [BsonElement(nameof(Chardonnay))]
            public int Chardonnay { get; set; }
            [BsonElement(nameof(PinotMeunier))]
            public int PinotMeunier { get; set; }
            [BsonElement(nameof(PinotNoir))]
            public int PinotNoir { get; set; }
            [BsonElement(nameof(AlchoholVol))]
            public double AlcoholVol { get; set; }
            [BsonElement(nameof(RedWineAmount))]
            public double RedWineAmount { get; set; }
            
            [BsonElement(nameof(Count))]
            public double Count { get; set; }
            [BsonElement(nameof(Sum))]
            public double Sum { get; set; }
            [BsonElement(nameof(AverageRating))]
            public double AverageRating { get; set; }
            
            [BsonElement(nameof(BlendInfo))]
            public string BlendInfo { get; set; }
            [BsonElement(nameof(Appearance))]
            public string Appearance { get; set; }
            [BsonElement(nameof(Dosage))]
            public string Dosage { get; set; }
            [BsonElement(nameof(Taste))]
            public string Taste { get; set; }
            [BsonElement(nameof(VintageYear))]
            public string VintageYear { get; set; }
            [BsonElement(nameof(Aroma))]
            public string Aroma { get; set; }
            [BsonElement(nameof(Vintage))]
            public bool Vintage { get; set; }
            [BsonElement(nameof(DosageAmount))]
            public string DosageAmount { get; set; }
            
            public string[] Character { get; set; }
            
            public string[] Style { get; set; } 
        }

        public class CPData
        {
            [BsonId]
            public string Id { get; set; }
            [BsonElement(nameof(BrandName))]
            public string BrandName { get; set; }
            [BsonElement(nameof(AverageRating))]
            public double AverageRating { get; set; }
            [BsonElement(nameof(MasterId))]
            public string MasterId { get; set; }
            [BsonElement(nameof(Sum))]
            public double Sum { get; set; }
            [BsonElement(nameof(BottleName))]
            public string BottleName { get; set; }
            [BsonElement(nameof(BrandId))]
            public string BrandId { get; set; }
            [BsonElement(nameof(Count))]
            public double Count { get; set; }
            [BsonElement(nameof(CPId))]
            public string CPId { get; set; }
            [BsonElement(nameof(isMother))]
            public bool isMother { get; set; }
            [BsonElement(nameof(Year))]
            public int Year { get; set; }
            [BsonElement(nameof(Vintage))]
            public bool Vintage { get; set; }
            [BsonElement(nameof(BottleImg))]
            public string BottleImg { get; set; }
            [BsonElement(nameof(Dosage))]
            public string Dosage { get; set; }
            [BsonElement(nameof(Character))]
            public string[] Character { get; set; }
            [BsonElement(nameof(style))]
            public string[] style { get; set; }
        }

        private string ResolveDosage(Champagne context, string dosage)
        {
            if (dosage.Equals("Brut"))
            {
                return DosageEnum.Brut;
            }
            
            if (dosage.Equals("Demi-Sec"))
            {
                return DosageEnum.DemiSec;
            }

            if (dosage.Equals("Sec"))
            {
                return DosageEnum.Sec;
            }

            if (dosage.Equals("Extra-Brut"))
            {
                return DosageEnum.ExtraBrut;
            }

            if (dosage.Equals("Brut Nature"))
            {
                return DosageEnum.BrutNature;
            }

            if (dosage.Equals("Doux"))
            {
                return DosageEnum.Doux;
            }

            if (dosage.Equals("Extra Brut"))
            {
                return DosageEnum.ExtraBrut;
            }
            
            if (dosage.Equals("Extra-Dry"))
            {
                return DosageEnum.ExtraDry;
            }

            if (dosage.Equals("No dosage"))
            {
                WriteToErrorLog(context, "Old dosage: 'No dosage' --> Converted to Brut");
                return DosageEnum.Brut;
            }

            if (dosage.Equals("Zero"))
            {
                WriteToErrorLog(context, "Old dosage: 'Zero' --> Converted to Brut Nature");
                return DosageEnum.BrutNature;
            }

            if (dosage.Equals("Rosé"))
            {
                WriteToErrorLog(context, "Old dosage: 'Rosé' --> Converted to Brut");
                return DosageEnum.Brut;
            }

            if (dosage.Equals(""))
            {
                WriteToErrorLog(context, "Old dosage: ''(nothing) --> Converted to Brut");
                return DosageEnum.Brut;
            }

            return dosage;
        }

        private string ResolveStyle(Champagne context, string[] styles)
        {
            var list = new HashSet<string>();
            
            foreach (var style in styles)
            {
                if (style.Equals("Vintage"))
                {
                    WriteToErrorLog(context, "Old style: 'Vintage' --> Converted to TradBrut");
                    return ChampagneStyleEnum.Style.TradBrut.ToString();
                }
                if (style.Equals("Rosé"))
                {
                    return ChampagneStyleEnum.Style.Rose.ToString();
                }
                if (style.Equals("On Ice"))
                {
                    return ChampagneStyleEnum.Style.OnIce.ToString();
                }
                if (style.Equals("Blanc de Noirs"))
                {
                    return ChampagneStyleEnum.Style.BlancDeNoir.ToString();
                }
                if (style.Equals("Blanc de Blancs"))
                {
                    return ChampagneStyleEnum.Style.BlancDeBlanc.ToString();
                }
                if (style.Equals("Brut NV"))
                {
                    return ChampagneStyleEnum.Style.TradBrut.ToString();
                }
                if (style.Equals("Sweet"))
                {
                    return ChampagneStyleEnum.Style.TradSweet.ToString();
                }

                return "";
            }

            return "";
        }

        private string ResolveCharacter(Champagne context, string[] characters)
        {
            foreach (var character in characters)
            {
                if (character.Equals("Prestige Cuvée"))
                {
                    WriteToErrorLog(context, "Old character 'Prestige Cuvée' --> Converted to PremierCru");
                    return "PremierCru";
                }

                if (character.Equals("Cuvée"))
                {
                    WriteToErrorLog(context, "Old character 'Cuvée' --> Converted to GrandCru");
                    return "GrandCru";
                }

                if (character.Equals("Grand Cru"))
                {
                    return "GrandCru";
                }

                if (character.Equals("Premier Cru"))
                {
                    return "PremierCru";
                }
            }

            return "";
        }

        private string CreateChampagneMigrationLog()
        {
            var migrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/ChampagneMigration.txt";
            if (!File.Exists(migrationPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(migrationPath)) 
                {
                    sw.WriteLine("Champagne Migration Log - " + DateTime.Now);
                    sw.WriteLine("");
                }	 
            }
            else
            {
                //Clear previous migration log
                File.WriteAllText(migrationPath, "Champagne Migration Log - " + DateTime.Now + "\n");
            }

            return migrationPath;
        }

        private string CreateChampagneMigrationErrorLog()
        {
            var migrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/ChampagneMigrationErrorLog.txt";
            if (!File.Exists(migrationPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(migrationPath)) 
                {
                    sw.WriteLine("Champagne Migration Error Log - " + DateTime.Now);
                    sw.WriteLine("");
                }	 
            }
            else
            {
                //Clear previous migration log
                File.WriteAllText(migrationPath, "Champagne Migration Error Log - " + DateTime.Now + "\n");
            }

            return migrationPath;
        }

        private void WriteToErrorLog(Champagne champagne, string msg)
        {
            var master = _errorLog.Where(x => x.Key.Id.Equals(champagne.Id));

            if (master.Any())
            {
                _errorLog[champagne].Add(msg);
            }
            else
            {
                _errorLog.Add(champagne, new List<string>());
                _errorLog[champagne].Add(msg);
            }
        }

        private void FlushErrorLogToFile()
        {
            var migrationErr = CreateChampagneMigrationErrorLog();
            // Create a file to write to.
            var migrationPath = Directory.GetCurrentDirectory() + "/MigrationLogs/ChampagneMigrationErrorLog.txt";

            using (StreamWriter sw = File.AppendText(migrationPath)) 
            {
                foreach (var err in _errorLog)
                {
                    var champagne = err.Key;
                    var newId = Guid.Empty;
                    if (_champagneIdConvertion.Any(x => x.Key.Equals(champagne.Id)))
                    {
                        newId = _champagneIdConvertion[champagne.Id];
                    }
                    
                    sw.WriteLine($"{champagne.BottleName} - OldId:{champagne.Id} - NewId:{newId} \n"); //Make this champagne info
                    foreach (var val in err.Value)
                    {
                       sw.WriteLine("\t--> " +val ); 
                    }
                    sw.WriteLine("\n");
                }
                sw.WriteLine("Champagne Migration Error Log - " + DateTime.Now);
                sw.WriteLine("");
            }
        }
    }
}


