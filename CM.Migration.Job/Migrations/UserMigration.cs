using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using CM.Backend.API.Controllers;
using CM.Backend.API.Helpers;
using CM.Backend.API.RequestModels.UserRequestModels;
using CM.Backend.Commands.Commands.BrandCommands;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.Configuration;
using CM.Backend.Persistence.Model.MigrationSource;
using CM.Backend.Queries.Queries.MigrationSourceQueries;
using CM.Backend.Queries.Queries.UserCreationQuerires;
using EmailValidation;
using Marten.Util;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using SimpleSoft.Mediator;

namespace CM.Migration.Job.Migrations
{
    public class UserMigration
    {
        private Dictionary<string, Guid> _userIdConvertion;
        
        
        private readonly IMongoDatabase _source;
        private readonly FileHelper _fileHelper;
        private readonly ICommandRouter _commandRouter;
        private readonly IQueryRouter _queryRouter;
        private readonly IOptions<ProjectionsPersistenceConfiguration> _config;
        private readonly UserController _userController;

        private IMongoClient client;
        private IMongoDatabase database;
        private IMongoCollection<UserModel> defaultCollection;

        private UpdateDefinitionBuilder<UserModel> Update => Builders<UserModel>.Update;
	    
        private string collectionName { get; set; } = "Users";

        private ErrorLogging<User> _Log;
        
        public UserMigration(IMongoDatabase source, FileHelper fileHelper, ICommandRouter commandRouter, IQueryRouter queryRouter, IOptions<ProjectionsPersistenceConfiguration> config, UserController userController)
        {
            _source = source;
            _fileHelper = fileHelper;
            _commandRouter = commandRouter;
            _queryRouter = queryRouter;
            _config = config;
            _userController = userController;

            _userIdConvertion = new Dictionary<string, Guid>();

            //Configure database connection
            client = new MongoClient(config.Value.MongoClusterConnectionString);
            database = client.GetDatabase("identity_userrepo");
            defaultCollection = database.GetCollection<UserModel>(collectionName);

            //Check if the repository exists
            var filter = new BsonDocument("name", collectionName);
            var collections = database.ListCollections(new ListCollectionsOptions() {Filter = filter});

            if (!collections.Any())
            {
                database.CreateCollection(collectionName);
            }
        }

        public async Task Execute()
        {
            _Log = new ErrorLogging<User>("User", "User Migration");
            
            Console.WriteLine("Do you want to continue the UserMigration procedure? Press enter to proceed");
            //var input = Console.In.Read();
            
            var convertedUser = 0;
            
            Console.WriteLine("Pre-fetching migration resources...");
            
            var coll = _source.GetCollection<User>("_User");
            var users = coll.FindSync(FilterDefinition<User>.Empty).ToList();
            
            Console.WriteLine("Migration resources fetched... Migrating Users...");
            //Remember to turn off emails and push-notifications when migrating
            foreach (var user in users)
            {
                //Query to see if the this user has already been migrated.
                var migrationSource = await _queryRouter.QueryAsync<GetMigrationSourceBySourceId, MigrationSource>(
                    new GetMigrationSourceBySourceId(user.Id));

                if (migrationSource != null)
                {
                    _Log.WriteToConvertionLog(user.Id, migrationSource.Id, $"User migrated: {user.email} old Id: {user.Id} --> new Id: {migrationSource.Id} --> $${user.Id} ; $${migrationSource.Id}");
                    Console.WriteLine($"{user.email} - OldId:{user.Id} - NewId:{migrationSource.Id} --> User already migrated");
                    convertedUser++;
                    Console.WriteLine("******************************");
                    Console.WriteLine("Converted " + convertedUser + "/" + users.Count);
                    Console.WriteLine("******************************");
                    continue;
                }
                
                //Validate Email
                if (!EmailValidator.Validate(user.email))
                {
                    var fixedEmail = TryFixEmail(user.email);
                    if (!EmailValidator.Validate(fixedEmail))
                    {
                        _Log.WriteToErrorLog(user,
                            $"User oldId:{user.Id} Email:{user.email} --> User has an invalid email and could therefor not be recreated");
                        Console.WriteLine(
                            $"User oldId:{user.Id} Email:{user.email} --> User has an invalid email and could therefor not be recreated");
                        continue;
                    }

                    user.email = fixedEmail;
                }
                
                //Migrate User in backend!
                var backendMigrationResult = await _commandRouter.RouteAsync<MigrateUser, IdResponse>(new MigrateUser(
                    new CreateUser(
                        user.email.ToLower(),
                        user.username,
                        user.ProfileName,
                        "",
                        DateTime.UtcNow.Ticks), MigrationSourceEnum.OldLive.ToString(), user.Id));

                if (!backendMigrationResult.IsSuccessful)
                {
                    if (backendMigrationResult.Id.Equals(Guid.Empty))
                    {
                        _Log.WriteToErrorLog(user, $"User oldId:{user.Id} - Email:{user.email} --> Failed to be created in backend, msg: {backendMigrationResult.Message}");
                        continue; 
                    }
                    else
                    {
                        Console.WriteLine("User already migrated!");
                        _Log.WriteToConvertionLog(user.Id, backendMigrationResult.Id,
                            $"User migrated: {user.email} old Id: {user.Id} --> new Id: {backendMigrationResult.Id} --> $${user.Id} ; $${backendMigrationResult.Id}");
                        continue;
                    }
                }
                
                Console.WriteLine("User migrated into backend --> Requesting creation on identityServer");
                //Insert migrated user in identityServer
                await InsertUserInIdentity(user, backendMigrationResult.Id, user.email, user._hashed_password);
                Console.WriteLine("User succesfully created --> Migrating Image 0%");

                var imageId = Guid.Empty;
                if (user.ProfileImg.Contains("UserImg.jpg"))
                {
                    //Download image and upload!
                    Console.WriteLine("Downloading user images");
                    byte[] userImgFile = null;
                    ImageUploadResponse imageResult = new ImageUploadResponse(false, Guid.Empty);
                    try
                    {
                        userImgFile = await _fileHelper.DownloadFile(user.ProfileImg);
                        /*imageResult = await _userController.UploadUserImages(backendMigrationResult.Id,
                            new FormFile(new MemoryStream(userImgFile), 0, userImgFile.Length, "ProfileImage",
                                user.ProfileImg));*/
                    }
                    catch (Exception ex)
                    {
                        _Log.WriteToErrorLog(user, $"Failed to migrate image for user: Msg: {ex.Message}");
                    }

                    if (imageResult.IsSuccesfull)
                    {
                        Console.WriteLine("Update user with downloaded image");
                        var updateCmd = new UpdateUserSettings(backendMigrationResult.Id);
                        updateCmd.ProfileImageId = imageResult.ImageId;
                        var updateImgResult = await _commandRouter.RouteAsync<UpdateUserSettings, Response>(updateCmd);

                        if (!updateImgResult.IsSuccessful)
                        {
                            _Log.WriteToErrorLog(user, "Downloaded image but failed to update user");
                        }
                        else
                        {
                            Console.WriteLine("User image successfully migrated");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"User --> OldId:{user.Id} - NewId:{backendMigrationResult.Id} --> Failed to migrate image");
                        _Log.WriteToErrorLog(user, "Failed to migrate image");
                    }
                }
                else
                {
                    Console.WriteLine("No user image has been uploaded! Image migration complete --> Continuing");
                }
                
                
                //Log user migration to file
                _Log.WriteToConvertionLog(user.Id, backendMigrationResult.Id,
                    $"User migrated: {user.email} old Id: {user.Id} --> new Id: {backendMigrationResult.Id} --> $${user.Id} ; $${backendMigrationResult.Id}");
                
                convertedUser++;
                Console.WriteLine("******************************");
                Console.WriteLine("Converted " + convertedUser + "/" + users.Count);
                Console.WriteLine("******************************");

                
            }
            _Log.FlushErrorLogToFile("");
        }
        
        public class User : TEntity
        {
            [BsonId]
            public string Id { get; set; }
            [BsonElement(nameof(gender))]
            public string gender { get; set; }
            [BsonElement(nameof(brands))]
            public string[] brands { get; set; }
            [BsonElement(nameof(ProfileName))]
            public string ProfileName { get; set; }
            [BsonElement(nameof(Age))]
            public string Age { get; set; }
            [BsonElement(nameof(ProfileImg))]
            public string ProfileImg { get; set; }
            [BsonElement(nameof(Niveau))]
            public double Niveau { get; set; }
            [BsonElement(nameof(email))]
            public string email { get; set; }
            [BsonElement(nameof(username))]
            public string username { get; set; }
            [BsonElement(nameof(_hashed_password))]
            public string _hashed_password { get; set; }
        }

        public class UserModel
        {
            [BsonId]
            public Guid Id { get; set; }
		
            [BsonElement(nameof(Email))]
            public string Email { get; set; }

            [BsonElement(nameof(Password))]
            public string Password { get; set; }

            [BsonElement(nameof(IsActive))]
            public bool IsActive { get; set; }

            [BsonElement(nameof(Claims))]
            public List<string> Claims { get; set; }
        }


        private async Task InsertUserInIdentity(User user, Guid id, string email, string password)
        {
            //Check if the email already exists!
            var result = await defaultCollection.Find(x => x.Email == email).ToListAsync();

            if (result.Any())
            {
                //Log that this is a problem <--
                _Log.WriteToErrorLog(user, $"User oldId:{user.Id} --> newId:{id} -> Email:{user.email} --> Couldn't be migrated since hes email already exist in the userstore");
            }
            else
            {
                //Insert user in identity
                await defaultCollection.InsertOneAsync(new UserModel
                {
                    Id = id,
                    Password = password,
                    Email = email.ToLower(),
                    Claims = new List<string>(),
                    IsActive = true
                });
            }
        }

        private string TryFixEmail(string email)
        {
            if (email.Contains("mailto:"))
            {
                return email.Replace("mailto:", "");
            }

            if (email.Equals("thonycaleb@icloud"))
            {
                return email + ".com";
            }

            if (email.Equals("olivermarkharrison@gmail"))
            {
                return email + ".com";
            }

            return email;
        }
    }
}