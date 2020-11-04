using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.UserCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Brand;
using CM.Backend.Domain.Aggregates.Brand.ValueObjects;
using CM.Backend.Domain.Aggregates.Shared;
using CM.Backend.Domain.Aggregates.User;
using CM.Backend.Domain.Aggregates.User.Commands;
using CM.Backend.Domain.Aggregates.User.Entities;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.Exceptions;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Marten.Linq.SoftDeletes;
using Serilog;
using SimpleSoft.Mediator;
using BookmarkChampagne = CM.Backend.Commands.Commands.UserCommands.BookmarkChampagne;
using ConfirmEmail = CM.Backend.Commands.Commands.UserCommands.ConfirmEmail;
using CreateUser = CM.Backend.Commands.Commands.UserCommands.CreateUser;
using DeleteUser = CM.Backend.Commands.Commands.UserCommands.DeleteUser;
using DeregisterDeviceInstallation = CM.Backend.Commands.Commands.UserCommands.DeregisterDeviceInstallation;
using EditUserInfo = CM.Backend.Commands.Commands.UserCommands.EditUserInfo;
using EditUserSettings = CM.Backend.Commands.Commands.UserCommands.EditUserSettings;
using FollowBrand = CM.Backend.Commands.Commands.UserCommands.FollowBrand;
using FollowUser = CM.Backend.Commands.Commands.UserCommands.FollowUser;
using LikeEntity = CM.Backend.Commands.Commands.UserCommands.LikeEntity;
using MarkLatestNotificationSeen = CM.Backend.Commands.Commands.UserCommands.MarkLatestNotificationSeen;
using MarkNotificationAsRead = CM.Backend.Commands.Commands.UserCommands.MarkNotificationAsRead;
using RegisterDeviceInstallation = CM.Backend.Commands.Commands.UserCommands.RegisterDeviceInstallation;
using ResendConfirmationEmail = CM.Backend.Commands.Commands.UserCommands.ResendConfirmationEmail;
using UnbookmarkChampagne = CM.Backend.Commands.Commands.UserCommands.UnbookmarkChampagne;
using UnfollowBrand = CM.Backend.Commands.Commands.UserCommands.UnfollowBrand;
using UnfollowUser = CM.Backend.Commands.Commands.UserCommands.UnfollowUser;
using UnlikeEntity = CM.Backend.Commands.Commands.UserCommands.UnlikeEntity;
using UpdateProfileCover = CM.Backend.Commands.Commands.UserCommands.UpdateProfileCover;
using UpdateUserEmail = CM.Backend.Commands.Commands.UserCommands.UpdateUserEmail;
using UpdateUserNotificationSettings = CM.Backend.Commands.Commands.UserCommands.UpdateUserNotificationSettings;

namespace CM.Backend.Commands.Handlers
{
    public class UserHandler : CommandHandlerBase,
        ICommandHandler<CreateUser, IdResponse>,
        ICommandHandler<EditUserInfo, Response>,
        ICommandHandler<EditUserSettings, Response>,
        ICommandHandler<UpdateProfileCover, Response>,
        ICommandHandler<UpdateProfileImage, Response>,
        ICommandHandler<UpdateCellarCardImage, Response>,
        ICommandHandler<UpdateCellarHeaderImage, Response>,
        ICommandHandler<FollowUser, Response>,
        ICommandHandler<UnfollowUser, Response>,
        ICommandHandler<FollowBrand, Response>,
        ICommandHandler<UnfollowBrand, Response>,
        ICommandHandler<BookmarkChampagne, Response>,
        ICommandHandler<UnbookmarkChampagne, Response>,
        ICommandHandler<LikeEntity, Response>,
        ICommandHandler<UnlikeEntity, Response>,
        ICommandHandler<UpdateUserSettings, Response>,
        ICommandHandler<UpdateUserNotificationSettings, Response>,
        ICommandHandler<RegisterDeviceInstallation, Response>,
        ICommandHandler<DeregisterDeviceInstallation, Response>,
        ICommandHandler<MarkNotificationAsRead, Response>,
        ICommandHandler<MarkLatestNotificationSeen, Response>,
        ICommandHandler<UpdateUserTimeZone, Response>,
        ICommandHandler<UpdateUserEmail, Response>,
        ICommandHandler<ConfirmEmail, Response>,
        ICommandHandler<ResendConfirmationEmail, Response>,
        ICommandHandler<DeleteUser, Response>,
        ICommandHandler<MigrateUser, IdResponse>

    {
        public UserHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }

        public async Task<IdResponse> HandleAsync(CreateUser cmd, CancellationToken ct)
        {
            var user = new User();

            user.Execute(new Domain.Aggregates.User.Commands.CreateUser(
                new AggregateId(Guid.NewGuid()),
                new Email(cmd.Email),
                new Name(cmd.Name),
                cmd.ProfileName,
                cmd.Biography,
                new SleepSettings(new UTCOffSet(cmd.UtcOffset)), 
                new ImageId(Guid.Empty),
                new ImageId(Guid.Empty),
                new ImageId(Guid.Empty),
                new ImageId(Guid.Empty),
                null,
                null,
                false));

            user.Execute(new Domain.Aggregates.User.Commands.AddNotificationsSettings(true,true,true,true,true,true,true,true,true));

            await AggregateRepo.StoreAsync(user);
            Logger.Information("New {@User} created", user);

            return new IdResponse(user.Id);

        }

        public async Task<Response> HandleAsync(EditUserInfo cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.Id);

            user.Execute(new Domain.Aggregates.User.Commands.EditUserInfo(new Name(cmd.Name), cmd.ProfileName, cmd.Biography));

            await AggregateRepo.StoreAsync(user);


            return Response.Success();

        }

        public async Task<Response> HandleAsync(EditUserSettings cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.Id);

            user.Execute(new Domain.Aggregates.User.Commands.EditUserSettings(new CountryCode(cmd.CountryCode), new Language(cmd.Language)));

            await AggregateRepo.StoreAsync(user);

            return Response.Success();

        }

        public async Task<Response> HandleAsync(UpdateProfileCover cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            user.Execute(new Domain.Aggregates.User.Commands.UpdateProfileCover(new ImageId(cmd.ProfileCoverImg)));

            await AggregateRepo.StoreAsync(user);

            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateProfileImage cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            user.Execute(new Domain.Aggregates.User.Commands.UpdateProfileImg(new ImageId(cmd.ProfileImgId)));

            await AggregateRepo.StoreAsync(user);

            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateCellarCardImage cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            user.Execute(new Domain.Aggregates.User.Commands.UpdateCellarCardImg(new ImageId(cmd.CellarCardImgId)));

            await AggregateRepo.StoreAsync(user);

            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateCellarHeaderImage cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            user.Execute(new Domain.Aggregates.User.Commands.UpdateCellarHeaderImg(new ImageId(cmd.CellarHeaderImgId)));

            await AggregateRepo.StoreAsync(user);

            return Response.Success();

        }

        public async Task<Response> HandleAsync(FollowUser cmd, CancellationToken ct)
        {
            var followByUser = await AggregateRepo.LoadAsync<User>(cmd.FollowById);
            var followToUser = await AggregateRepo.LoadAsync<User>(cmd.FollowToId);

            //Since the user invocating this action (followByUser) already contains a follow reference to the user(followerToUser)
            //Communicate that everything is fine and end the call here...
            if (followByUser.Following.Contains(new AggregateId(cmd.FollowToId)))
            {
                return new Response(true);
            }
            
            followByUser.Execute(new Domain.Aggregates.User.Commands.FollowUser(new AggregateId(cmd.FollowToId), new NotEmptyString(followToUser.Name.Value), followToUser.ImageCustomization.ProfilePictureImgId));

            await AggregateRepo.StoreAsync(followByUser);

            return new Response(true);
        }

        public async Task<Response> HandleAsync(UnfollowUser cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.FollowById);

            //if the user whom is invocating the unfollow action does not have a refference to the person they are about to unfollow
            //in there collection of people they follow, communicate that everything is in order
            if(!user.Following.Contains(new AggregateId(cmd.FollowToId)))
            {
                return new Response(true);
            }

            user.Execute(new Domain.Aggregates.User.Commands.UnfollowUser(new AggregateId(cmd.FollowToId)));

            await AggregateRepo.StoreAsync(user);

            return new Response(true);
        }

        public async Task<Response> HandleAsync(FollowBrand cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);

             //If the user invoking this method already has a reference to the brand, indicate that all is fine
             if(user.FollowingBrands.Contains(new AggregateId(cmd.BrandId)))
            {
                return new Response(true);
            }

            user.Execute(new Domain.Aggregates.User.Commands.FollowBrand(new AggregateId(cmd.BrandId), new BrandName(brand.Name.Value), brand.LogoImageId));

            await AggregateRepo.StoreAsync(user);

            return new Response(true);

        }

        public async Task<Response> HandleAsync(UnfollowBrand cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            //If the user invoking this method no longer has a reference to the brand he/she is about to unfollow return everything is fine
            if(!user.FollowingBrands.Contains(new AggregateId(cmd.BrandId)))
            {
                return new Response(true);
            }

            user.Execute(new Domain.Aggregates.User.Commands.UnfollowBrand(new AggregateId(cmd.BrandId)));

            await AggregateRepo.StoreAsync(user);

            return new Response(true);
        }

        public async Task<Response> HandleAsync(BookmarkChampagne cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            //If the user, bookmarking this champagne already has a reference if the bookmarked list return that everything is fine
            if(user.BookmarkedChampagnes.Contains(new AggregateId(cmd.ChampagneId)))
            {
                return new Response(true);
            }

            user.Execute(new Domain.Aggregates.User.Commands.BookmarkChampagne(new AggregateId(cmd.ChampagneId)));

            await AggregateRepo.StoreAsync(user);

            return new Response(true);
        }

        public async Task<Response> HandleAsync(UnbookmarkChampagne cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            //If the user does not have a refference to the champagne in the list of bookmarked there is no reason to try again. Everything is in the desired state
            if(!user.BookmarkedChampagnes.Contains(new AggregateId(cmd.ChampagneId)))
            {
                return new Response(true);
            }

            user.Execute(new Domain.Aggregates.User.Commands.UnbookmarkChampagne(new AggregateId(cmd.ChampagneId)));

            await AggregateRepo.StoreAsync(user);

            return new Response(true);
        }

        public async Task<Response> HandleAsync(LikeEntity cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.LikeById);

            //If the user already have a referrence to the this entity they are already liking it, return true response
            if (user.LikedEntities.Contains(new AggregateId(cmd.LikeToContextId)))
            {
                return new Response(true);
            }
            
            user.Execute(new Domain.Aggregates.User.Commands.LikeEntity(new AggregateId(cmd.LikeToContextId), new LikeContextType(cmd.ContextType)));

            await AggregateRepo.StoreAsync(user);
            
            return new Response(true);
        }

        public async Task<Response> HandleAsync(UnlikeEntity cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.LikeById);

            //If the user does not have a refference to the entity there is no reason to invoke method return true
            if (!user.LikedEntities.Contains(new AggregateId(cmd.LikeToContextId)))
            {
                return new Response(true);
            }
            
            user.Execute(new Domain.Aggregates.User.Commands.UnlikeEntity(new AggregateId(cmd.LikeToContextId)));

            await AggregateRepo.StoreAsync(user);
            
            return new Response(true);
        }

        public async Task<Response> HandleAsync(UpdateUserSettings cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            //Check if userInfo should be edited
            if (cmd.Name != null || cmd.ProfileName != null || cmd.Biography != null)
            {
                var editUserInfoCmd =
                    new Domain.Aggregates.User.Commands.EditUserInfo(user.Name, user.ProfileName, user.Biography);

                if (cmd.Name != null)
                {
                    editUserInfoCmd.Name = new Name(cmd.Name);
                }

                if (cmd.ProfileName != null)
                {
                    editUserInfoCmd.ProfileName = cmd.ProfileName;
                }

                if (cmd.Biography != null)
                {
                    editUserInfoCmd.Biography = cmd.Biography;
                }
                
                user.Execute(editUserInfoCmd);
            }
            
            //Check if images should be updated!
            if (!cmd.ProfileImageId.Equals(Guid.Empty) || !cmd.ProfileCoverImageId.Equals(Guid.Empty) ||
                !cmd.ProfileCellarCardImageId.Equals(Guid.Empty))
            {
                var editUserImageCustomizationCmd = new Domain.Aggregates.User.Commands.UpdateUserImageCustomization(user.ImageCustomization.ProfilePictureImgId, user.ImageCustomization.ProfileCoverImgId, user.ImageCustomization.CellarCardImgId, user.ImageCustomization.CellarHeaderImgId);

                if (!cmd.ProfileImageId.Equals(Guid.Empty))
                {
                    editUserImageCustomizationCmd.ProfileImageId = new ImageId(cmd.ProfileImageId);
                }

                if (!cmd.ProfileCoverImageId.Equals(Guid.Empty))
                {
                    editUserImageCustomizationCmd.ProfileCoverImageId = new ImageId(cmd.ProfileCoverImageId);
                }

                if (!cmd.ProfileCellarCardImageId.Equals(Guid.Empty))
                {
                    editUserImageCustomizationCmd.ProfileCellarCardImageId = new ImageId(cmd.ProfileCellarCardImageId);
                }
                
                user.Execute(editUserImageCustomizationCmd);
            }

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateUserNotificationSettings cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            user.Execute(new Domain.Aggregates.User.Commands.UpdateUserNotificationSettings(cmd.ReceiveCMNotifications, cmd.NotifyNewFollower, cmd.NotifyNewComment, cmd.NotifyActivityInThread, cmd.NotifyLikeTasting, cmd.NotifyLikeComment, cmd.ReceiveNewsLetter, cmd.NotifyChampagneTasted, cmd.NotifyBrandNews));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }
        
        public async Task<Response> HandleAsync(RegisterDeviceInstallation cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            //Check if deviceInstallation exists!

            if (user.DeviceInstallations != null)
            {
                foreach (var deviceInstallation in user.DeviceInstallations)
                {
                    if (deviceInstallation.PushChannel.Value.Equals(cmd.PushChannel))
                    {
                        if (!deviceInstallation.Platform.Value.Equals(cmd.NotificationPlatform))
                        {
                            throw new DomainException("UserHandler.HandleAsync(RegisterDeviceInstallation) -> Exception () -> Can't update existing deviceInstallation with simirlar pushChannel: "+ cmd.PushChannel+ " because the notificationPlatform is not the same -> oldValue: " + deviceInstallation.Platform + " ; newValue: "+ cmd.NotificationPlatform + ". If you want to register a pushChannel with a different platform you should deregister the pushChannel if it already exists");
                        }
                    }
                }
            }
            
            user.Execute(new Domain.Aggregates.User.Commands.RegisterDeviceInstallation(new PushChannel(cmd.PushChannel), new DevicePlatform(cmd.NotificationPlatform)));
            
            await AggregateRepo.StoreAsync(user);

            return Response.Success();

        }

        public async Task<Response> HandleAsync(DeregisterDeviceInstallation cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            user.Execute(new Domain.Aggregates.User.Commands.DeregisterDeviceInstallation(new PushChannel(cmd.PushChannel)));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(MarkNotificationAsRead cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            user.Execute(new Domain.Aggregates.User.Commands.MarkNotificationAsRead(new AggregateId(cmd.NotificationId)));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(MarkLatestNotificationSeen cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            user.Execute(new Domain.Aggregates.User.Commands.MarkLatestNotificationSeen(new AggregateId(cmd.NotificationId)));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateUserTimeZone cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            user.Execute(new UpdateSleepSettings(new UTCOffSet(cmd.UTCOffset), user.UserSettings.SleepSettings.NotifyFrom, user.UserSettings.SleepSettings.NotifyTo));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(UpdateUserEmail cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);
            
            user.Execute(new Domain.Aggregates.User.Commands.UpdateUserEmail(new Email(cmd.Email)));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(ConfirmEmail cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            if (!user.Email.Value.Equals(cmd.Email))
            {
                return new Response(false, null, "The email confirmation link is related to email: " + cmd.Email + ". While the users current email is: " + user.Email);
            }
            
            user.Execute(new Domain.Aggregates.User.Commands.ConfirmEmail(new Email(cmd.Email), true));

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(ResendConfirmationEmail cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            if (user.UserSettings.IsEmailVerified) //TODO : LOG here because this means that he had the opportunity to send and resend even though he had already confiremd. RM not updated properly
            {
                Logger.Fatal("Email confirmation error. {@Command} executed even though the email was already confirmed on {@User}. RM may not be properly updated", cmd, user);
                return Response.Unsuccessful("The current email has already been confirmed");
            }
            
            user.Execute(new Domain.Aggregates.User.Commands.ResendConfirmationEmail());

            await AggregateRepo.StoreAsync(user);

            return Response.Success();
            
        }

        public async Task<Response> HandleAsync(DeleteUser cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.UserId);

            if (user.IsDeleted)
            {
                return new Response(true, null, "User is already deleted");
            }
            
            user.Execute(new Domain.Aggregates.User.Commands.DeleteUser());

            await AggregateRepo.StoreAsync(user);
            
            return Response.Success();
        }

        public async Task<IdResponse> HandleAsync(MigrateUser cmd, CancellationToken ct)
        {
            //Create User --> And Set migrationSource
            var user = new User();

            user.Execute(new Domain.Aggregates.User.Commands.CreateUser(
                new AggregateId(Guid.NewGuid()),
                new Email(cmd.CreateUserCmd.Email),
                new Name(cmd.CreateUserCmd.Name),
                cmd.CreateUserCmd.ProfileName,
                cmd.CreateUserCmd.Biography,
                new SleepSettings(new UTCOffSet(0)), 
                new ImageId(Guid.Empty),
                new ImageId(Guid.Empty),
                new ImageId(Guid.Empty),
                new ImageId(Guid.Empty),
                null,
                null,
                false));

            user.Execute(new Domain.Aggregates.User.Commands.AddNotificationsSettings(true,true,true,true,true,true,true,true,true));
            
            user.Execute(new SetMigrationSource(new MigrationSource(cmd.MigrationSource, cmd.SourceId)));
            
            await AggregateRepo.StoreAsync(user);
            Logger.Information("User Migrated {@Command}, {@User} created", cmd, user);
            
            return new IdResponse(user.Id);
        }
    }
}