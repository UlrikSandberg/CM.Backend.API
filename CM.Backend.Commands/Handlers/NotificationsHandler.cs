using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;
using CM.Backend.Commands.Commands.NotificationCommands;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Brand;
using CM.Backend.Domain.Aggregates.Champagne;
using CM.Backend.Domain.Aggregates.Comment;
using CM.Backend.Domain.Aggregates.Notification;
using CM.Backend.Domain.Aggregates.Notification.Commands;
using CM.Backend.Domain.Aggregates.Notification.ValueObjects;
using CM.Backend.Domain.Aggregates.Tasting;
using CM.Backend.Domain.Aggregates.User;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.EnumOptions;
using CM.Backend.Domain.SharedValueObjects;
using CM.Backend.Eventstore.Persistence;
using Marten.Linq;
using MongoDB.Driver.Core.WireProtocol.Messages.Encoders.BinaryEncoders;
using Newtonsoft.Json.Serialization;
using Serilog;
using SimpleSoft.Mediator;
using StructureMap.Pipeline;

namespace CM.Backend.Commands.Handlers
{
    public class NotificationsHandler : CommandHandlerBase,
        ICommandHandler<UserFollowedNotification, Response>,
        ICommandHandler<TastingCommentedNotification, Response>,
        ICommandHandler<TastingLikedNotification, Response>,
        ICommandHandler<CommentLikedNotification, Response>,
        ICommandHandler<ChampagneTastedNotification, Response>,
        ICommandHandler<ActivityInCommentThreadNotification, Response>,
        ICommandHandler<BrandNewsNotification, Response>,
        ICommandHandler<CommunityUpdateNotification, Response>,
        ICommandHandler<UserCreatedNotification, Response>
    {
        //Base notification type string
        private const string BaseNotificationType = "BaseNotification";
        
        //Placeholders
        private const string ChampagneNamePlaceholder = "{ChampagneName}";
        private const string UsernamePlaceholder = "{InvokedByName}";
        private const string BrandNamePlaceholder = "{BrandName}";
        private const string BroadCastPlaceholder = "{BroadCastMessage}";
        private const string GenericNamePlaceholder = "{GenericName}";
        
        //Notification standard messages
        private const string UserFollowedMessage = 
            UsernamePlaceholder + " started to follow you.";

        private const string TastingCommentedMessage =
            UsernamePlaceholder + " commented on your " + ChampagneNamePlaceholder + " tasting. Click here to see the new comment.";

        private const string TastingLikedMessage =
            UsernamePlaceholder + " liked your " + ChampagneNamePlaceholder + " tasting.";

        private const string CommentLikedMessage =
            UsernamePlaceholder + " liked your comment on " + ChampagneNamePlaceholder;

        private const string ChampagneTastedMessage =
            UsernamePlaceholder + " has tasted " + ChampagneNamePlaceholder + ". Click to see more here...";

        private const string ActivityInCommentThreadMessage =
            UsernamePlaceholder + " commented on " + ChampagneNamePlaceholder + " which you also commented.";

        private const string WelcomeMessage = 
            "Dear " + UsernamePlaceholder + " we want to welcome you to Champagne Moments, your passport to champagne.";
           
        
        public NotificationsHandler(IPublishingAggregateRepository aggregateRepo, ILogger logger) : base(aggregateRepo, logger)
        {
        }

        public async Task<Response> HandleAsync(UserFollowedNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();
            var placeholderDictionary = new Dictionary<string, string>();
            
            placeholderDictionary.Add(UsernamePlaceholder, cmd.FollowByName);
            
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.UserFollowed,
                NotificationAction.UserAction,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.FollowById),
                new Name(cmd.FollowByName),
                new ImageId(cmd.FollowByImageId),
                null,
                UserFollowedMessage,
                UserFollowedMessage,
                placeholderDictionary,
                new HashSet<AggregateId> {new AggregateId(cmd.FollowToId)}));

            await AggregateRepo.StoreAsync(notification);

            Logger.Information("Created {@Notification} for {UserId}", notification, cmd.FollowToId);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(TastingCommentedNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();
            var placeholderDictionary = new Dictionary<string, string>();
            
            placeholderDictionary.Add(UsernamePlaceholder, cmd.CommentedByName);
            placeholderDictionary.Add(ChampagneNamePlaceholder, cmd.BottleName);
            
            var message = PrepareMessage(TastingCommentedMessage, ChampagneNamePlaceholder, cmd.BottleName);
            
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.TastingCommented,
                NotificationAction.UserAction,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.CommentedById),
                new Name(cmd.CommentedByName),
                new ImageId(cmd.CommentedByProfileImgId),
                null,
                message,
                TastingCommentedMessage,
                placeholderDictionary,
                new HashSet<AggregateId>{new AggregateId(cmd.TastingAuthorId)}, cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            
            Logger.Information("Created {@Notification} for {UserId}", notification, cmd.TastingAuthorId);
            
            return Response.Success();
        }

        private string PrepareMessage(string message, string placeholder, string placeholderContent)
        {
            var preparedString = message.Replace(placeholder, placeholderContent);

            return preparedString;
        }

        public async Task<Response> HandleAsync(TastingLikedNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();
            var placeholderDictionary = new Dictionary<string, string>();
            
            placeholderDictionary.Add(UsernamePlaceholder, cmd.InvokerName);
            placeholderDictionary.Add(ChampagneNamePlaceholder, cmd.BottleName);
            
            var message = PrepareMessage(TastingLikedMessage, ChampagneNamePlaceholder, cmd.BottleName);

            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.TastingLiked,
                NotificationAction.UserAction,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.InvokerId),
                new Name(cmd.InvokerName),
                new ImageId(cmd.InvokerProfileImgId),
                null,
                message,
                TastingLikedMessage,
                placeholderDictionary,
                new HashSet<AggregateId>{new AggregateId(cmd.TastingAuthorId)},
                cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            Logger.Information("Created {@Notification} for {UserId}", notification, cmd.TastingAuthorId);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(CommentLikedNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();
            var placeholderDictionary = new Dictionary<string, string>();
            
            placeholderDictionary.Add(UsernamePlaceholder, cmd.InvokerName);
            placeholderDictionary.Add(ChampagneNamePlaceholder, cmd.BottleName);
            
            var message = PrepareMessage(CommentLikedMessage, ChampagneNamePlaceholder, cmd.BottleName);
            
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.CommentLiked,
                NotificationAction.UserAction,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.InvokerId),
                new Name(cmd.InvokerName),
                new ImageId(cmd.InvokerProfileImgId),
                null,
                message,
                CommentLikedMessage,
                placeholderDictionary,
                new HashSet<AggregateId>{new AggregateId(cmd.CommentAuthorId)}, cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            Logger.Information("Created {@Notification} for {UserId}", notification, cmd.CommentAuthorId);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(ChampagneTastedNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();
            var placeholderDictionary = new Dictionary<string, string>();
            
            placeholderDictionary.Add(UsernamePlaceholder, cmd.InvokerName);
            placeholderDictionary.Add(ChampagneNamePlaceholder, cmd.BottleName);
            
            var message = PrepareMessage(ChampagneTastedMessage, ChampagneNamePlaceholder, cmd.BottleName);
            
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.ChampagneTasted,
                NotificationAction.UserAction,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.InvokerId),
                new Name(cmd.InvokerName),
                new ImageId(cmd.InvokerProfileImgId),
                null,
                message,
                ChampagneTastedMessage,
                placeholderDictionary,
                new HashSet<AggregateId>(), cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            Logger.Information("Created {@Notification}", notification);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(ActivityInCommentThreadNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();
            var placeholderDictionary = new Dictionary<string, string>();
            
            placeholderDictionary.Add(ChampagneNamePlaceholder, cmd.BottleName);
            placeholderDictionary.Add(UsernamePlaceholder, cmd.CommentedByName);
            
            var message = PrepareMessage(ActivityInCommentThreadMessage, ChampagneNamePlaceholder, cmd.BottleName);
            
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.ActivityInCommentThread,
                NotificationAction.UserAction,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.CommentedById),
                new Name(cmd.CommentedByName),
                new ImageId(cmd.CommentedByProfileImgId),
                null,
                message,
                ActivityInCommentThreadMessage,
                placeholderDictionary,
                new HashSet<AggregateId>{new AggregateId(cmd.TastingAuthorId)},
                cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            Logger.Information("Created {@Notification} for {UserId}", notification, cmd.TastingAuthorId);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(BrandNewsNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();

            var placeholderDictionary = new Dictionary<string, string>();
            var message = cmd.Message;
            
            if (!cmd.ChampagneId.Equals(Guid.Empty))
            {
                if (!message.Contains(ChampagneNamePlaceholder))
                {
                    return Response.Unsuccessful("The provided champagne name placeholder is invalid. Correct use would be -> " + ChampagneNamePlaceholder);
                }
                
                var champagne = await AggregateRepo.LoadAsync<Champagne>(cmd.ChampagneId);

                var champagneName = champagne.BottleName.Value;
                
                if (champagne.VintageInfo.IsVintage)
                {
                    champagneName += champagne.VintageInfo.VintageYear.Value;
                }

                placeholderDictionary.Add(ChampagneNamePlaceholder, champagneName);
                message = PrepareMessage(message, ChampagneNamePlaceholder, champagneName);
            }
            placeholderDictionary.Add(UsernamePlaceholder, cmd.BrandName);
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.BrandUpdate,
                NotificationAction.BrandNews,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.BrandId),
                new Name(cmd.BrandName),
                new ImageId(cmd.BrandLogoImageId),
                null,
                message,
                cmd.Message,
                placeholderDictionary,
                new HashSet<AggregateId>(),
                cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            Logger.Information("Created {@Notification} for {BrandId}", notification, cmd.BrandId);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(CommunityUpdateNotification cmd, CancellationToken ct)
        {
            var notification = new Notification();

            var placeholderDictionary = new Dictionary<string, string>();
            
            var message = cmd.Message;
            var broadCastMessage = cmd.BroadCastMessage;
            
            if (!cmd.BrandId.Equals(Guid.Empty))
            {
                if (!message.Contains(BrandNamePlaceholder))
                {
                    return Response.Unsuccessful("The provided BrandNamePlaceholder is invalid. Correct usage -> " + BrandNamePlaceholder);
                }
                
                var brand = await AggregateRepo.LoadAsync<Brand>(cmd.BrandId);

                message = PrepareMessage(message, BrandNamePlaceholder, brand.Name.Value);
                placeholderDictionary.Add(BrandNamePlaceholder, brand.Name.Value);

                if (!string.IsNullOrEmpty(broadCastMessage))
                {
                    if (broadCastMessage.Contains(BrandNamePlaceholder))
                    {
                        broadCastMessage = PrepareMessage(broadCastMessage, BrandNamePlaceholder, brand.Name.Value);
                    }
                }
            }

            if (cmd.Message.Contains(UsernamePlaceholder))
            {
                placeholderDictionary.Add(UsernamePlaceholder, GenericNamePlaceholder);
            }
            //Since this is a broadcast we will add an extra element to our placeholderDictionary being broadCastMessage;
            if (!string.IsNullOrEmpty(broadCastMessage))
            {
                placeholderDictionary.Add(BroadCastPlaceholder, broadCastMessage);
            }
            //TODO : I decided to keep the notification as they are right now, since they are working... So changes will have to be made but since the notification are easy to switch lets take it afterwards
            //TODO : I imagined a notifion design with som inheritance, something like -> BaseNotification --> CommunityUpdate --> BrandNews --> BroadCasting --> UserInvokedNotification --> Or something like this.
            //TODO : For-example i give a Guid.NewGuid(), here as invoker id... Since it is required by value objects... Not transparent there different notification structure could be used to solve this.
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.CommunityUpdate,
                NotificationAction.CommunityUpdate,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(Guid.NewGuid()), 
                new Name("Champagne Moments"),
                new ImageId(Guid.Empty),
                null,
                message,
                cmd.Message,
                placeholderDictionary,
                new HashSet<AggregateId>(), cmd.ContextUrl));

            await AggregateRepo.StoreAsync(notification);
            Logger.Information("Created {@Notification} for community update", notification);
            
            return Response.Success();
        }

        public async Task<Response> HandleAsync(UserCreatedNotification cmd, CancellationToken ct)
        {
            var user = await AggregateRepo.LoadAsync<User>(cmd.InvokerId);
            
            var notification = new Notification();
            
            var placeholderDictionary = new Dictionary<string, string>();

            var message = WelcomeMessage;
            
            notification.Execute(new CreateNotification(
                new NotificationType(BaseNotificationType),
                NotificationMethod.WelcomeMessage,
                NotificationAction.CommunityUpdate,
                DateTime.Now,
                DateTime.Now.AddDays(60),
                new AggregateId(cmd.InvokerId),
                new Name(user.Name.Value),
                new ImageId(Guid.Empty),
                null,
                message,
                message,
                placeholderDictionary,
                new HashSet<AggregateId>{new AggregateId(cmd.InvokerId)}));

            await AggregateRepo.StoreAsync(notification);
            
            return Response.Success();
        }
    }
}