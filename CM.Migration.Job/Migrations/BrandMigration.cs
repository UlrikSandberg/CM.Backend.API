using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.API.Controllers;
using CM.Backend.API.EnumOptions;
using CM.Backend.API.RequestModels;
using CM.Backend.API.RequestModels.BrandRequestModels;
using CM.Backend.Commands.Commands.BrandCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Queries.Queries.MigrationSourceQueries;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimpleSoft.Mediator;
using StructureMap.Diagnostics;

namespace CM.Migration.Job.Migrations
{
    public class BrandMigration
    {
        public Dictionary<string, Guid> IdConversions => new Dictionary<string, Guid>();
        private readonly IMongoDatabase _source;
        private readonly ICommandHandler<MigrateBrand, IdResponse> _brandHandler;
        private readonly BrandsController _brandsController;
        private readonly FileHelper _fileHelper;
        private readonly IQueryRouter _queryRouter;

        public BrandMigration(IMongoDatabase source, ICommandHandler<MigrateBrand, IdResponse> brandHandler, BrandsController brandsController, FileHelper fileHelper, IQueryRouter queryRouter)
        {
            _source = source;
            _brandHandler = brandHandler;
            _brandsController = brandsController;
            _fileHelper = fileHelper;
            _queryRouter = queryRouter;
        }

        public async Task Execute()
        {
            Console.WriteLine("Do you want to continue the BrandMigration procedure? Press enter to proceed");
            //var input = Console.In.Read();
            
            
            var coll = _source.GetCollection<BrandData>("BrandData");
            var brands = coll.FindSync(FilterDefinition<BrandData>.Empty).ToList();

            var convertedBrands = 0;
            
            //Create a log file!
            string txtPath = Directory.GetCurrentDirectory() + "/MigrationLogs/BrandMigration.txt";
            if (!File.Exists(txtPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(txtPath)) 
                {
                    sw.WriteLine("Brand Migration Log - " + DateTime.Now);
                    sw.WriteLine("");
                }	 
            }
            else
            {
                
                //Clear previous migration log
                File.WriteAllText(txtPath, "Brand Migration Log - " + DateTime.Now + "\n");
            }
            
            //Create a log file!
            string txtPathError = Directory.GetCurrentDirectory() + "/MigrationLogs/BrandMigrationErrorLog.txt";
            if (!File.Exists(txtPath))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(txtPath)) 
                {
                    sw.WriteLine("Brand Migration Error Log - " + DateTime.Now);
                    sw.WriteLine("");
                }	 
            }
            else
            {
                //Clear previous migration log
                File.WriteAllText(txtPath, "Brand Migration Error Log - " + DateTime.Now + "\n");
            }
            
            foreach (var brand in brands)
            {
                var migrationSource = await _queryRouter.QueryAsync<GetMigrationSourceBySourceId, MigrationSource>(
                    new GetMigrationSourceBySourceId(brand.Id));

                if (migrationSource != null)
                {
                    Console.WriteLine($"{brand.BrandSearchName} already migrated --> Continuing");
                    convertedBrands++;
                    Console.WriteLine("******************************");
                    Console.WriteLine("Converted "+convertedBrands + "/" + brands.Count);
                    Console.WriteLine("******************************");
                    
                    using (StreamWriter sw = File.AppendText(txtPath)) 
                    {
                        sw.WriteLine($"Brand migrated: {brand.BrandSearchName} old Id: {brand.Id} --> new Id: {migrationSource.Id} --> $${brand.Id} ; $${migrationSource.Id}");
                    }	
                    
                    continue;
                }
                
                Console.WriteLine("Starting migration of brand: " +brand.BrandSearchName);
                var newId = await _brandHandler.HandleAsync(new MigrateBrand(brand.BrandSearchName, "OldLive", brand.Id), CancellationToken.None);
                if (newId.IsSuccessful)
                {
                    IdConversions.Add(brand.Id, newId.Id);
                    Console.Out.WriteLine($"Brand migrated: {brand.BrandSearchName} old ID: {brand.Id} -> new ID: {newId.Id}");
                    //Write result to a local file!
                    // Create a file to write to.
                    using (StreamWriter sw = File.AppendText(txtPath)) 
                    {
                        sw.WriteLine($"Brand migrated: {brand.BrandSearchName} old Id: {brand.Id} --> new Id: {newId.Id} --> $${brand.Id} ; $${newId.Id}");
                    }	

                    var brandCoverFile = await _fileHelper.DownloadFile(brand.BrandCover);
                    var bottleCoverFile = await _fileHelper.DownloadFile(brand.BottleCover);
                    Console.WriteLine($"Uploading images for brand : {brand.BrandSearchName}... --> 0%");
                    var imageResult1 = await _brandsController.UploadImages(
                        new FormFile(new MemoryStream(brandCoverFile), 0, brandCoverFile.Length, "brandCover", brand.BrandCover),
                        newId.Id, $"brandCover{Path.GetExtension(brand.BrandCover)}", BrandTypes.TypeOfBrandImage.Cover, false) as ObjectResult;
                    Console.WriteLine("Image upload at 50%");
                    var imageResult2 = await _brandsController.UploadImages(
                        new FormFile(new MemoryStream(bottleCoverFile), 0, bottleCoverFile.Length, "bottleCover", brand.BottleCover),
                        newId.Id, $"bottleCover{Path.GetExtension(brand.BottleCover)}", BrandTypes.TypeOfBrandImage.Cover, false) as ObjectResult;
                    Console.WriteLine("Image upload at 100% --> Finishing Brand creation");

                    var brandCoverImageId = Guid.Empty;
                    var brandCoverParseResult = Guid.TryParse(imageResult1.Value.ToString(), out brandCoverImageId);
                    if (!brandCoverParseResult)
                    {
                        using (StreamWriter sw = File.AppendText(txtPathError)) 
                        {
                            sw.WriteLine($"{brand.BrandSearchName} - OldId:{brand.Id} - NewId:{newId} --> Failed to convert brandCoverImage");
                        }	 
                    }
                    
                    var bottleCoverImageId = Guid.Empty;
                    var bottleCoverParseResult = Guid.TryParse(imageResult2.Value.ToString(), out bottleCoverImageId);
                    if (!bottleCoverParseResult)
                    {
                        using (StreamWriter sw = File.AppendText(txtPathError)) 
                        {
                            sw.WriteLine($"{brand.BrandSearchName} - OldId:{brand.Id} - NewId:{newId} --> Failed to convert bottleCoverImage");
                        }	
                    }
                    
                    Console.WriteLine("UpdateBrandImages... Progress...");
                    var updateBrandImages = _brandsController.UpdateBrandImages(
                        newId.Id,
                        new UpdateBrandImagesRequest
                        {
                            BrandLogoImageId = Guid.Empty,
                            BottleCoverImageId = bottleCoverImageId,
                            BrandCoverImageId = brandCoverImageId,
                            BrandListImageId = brandCoverImageId
                        });
                    Console.WriteLine("UpdateBrandImages complete --> UpdateCellar... Progress...");
                    
                    var updateBrandCellar = _brandsController.UpdateCellar(
                        newId.Id,
                        new UpdateBrandCellarRequest
                        {
                            CardImgId = bottleCoverImageId,
                            HeaderImgId = bottleCoverImageId
                        });
                    Console.WriteLine("UpdateCellar complete. Brand : " + brand.BrandSearchName + " Succesfully created --> Publishing Brand.");

                    await _brandsController.SetPublishingStatusBrand(newId.Id,
                        BrandPublishingRequest.PublishingStatusEnum.Published);

                    convertedBrands++;
                    Console.WriteLine("******************************");
                    Console.WriteLine("Converted "+convertedBrands + "/" + brands.Count);
                    Console.WriteLine("******************************");

                    
                }
                else
                {
                    if (!newId.Equals(Guid.Empty))
                    {
                        //The call was not succesful but the empty was neither empty. This means that the brand was migrated before log this
                        Console.WriteLine($"{brand.BrandSearchName} - OldId:{brand.Id} - NewId:{newId.Id} --> Already migrated");
                        
                        using (StreamWriter sw = File.AppendText(txtPath)) 
                        {
                            sw.WriteLine($"Brand migrated: {brand.BrandSearchName} old Id: {brand.Id} --> new Id: {newId.Id} --> $${brand.Id} ; $${newId.Id}");
                        }	
                        
                        convertedBrands++;
                        Console.WriteLine("******************************");
                        Console.WriteLine("Converted "+convertedBrands + "/" + brands.Count);
                        Console.WriteLine("******************************");
                    }
                    else
                    {
                        Console.Out.WriteLine($"****** ERROR MIGRATING BRAND: {brand.Id} : {brand.BrandSearchName}");   
                    }
                }
            }
        }


        public class BrandData
        {
            [BsonId]
            public string Id { get; set; }
            [BsonElement(nameof(BrandSearchName))]
            public string BrandSearchName { get; set; }
            [BsonElement(nameof(BrandCover))]
            public string BrandCover { get; set; }
            [BsonElement(nameof(BottleCover))]
            public string BottleCover { get; set; }
            [BsonElement(nameof(NumberOfBottles))]
            public int NumberOfBottles { get; set; }
        }
    }
}