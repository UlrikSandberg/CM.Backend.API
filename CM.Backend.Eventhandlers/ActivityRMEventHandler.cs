using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Commands.Commands.NotificationCommands;
using CM.Backend.Documents.Messages;
using CM.Backend.Documents.Responses;
using CM.Backend.Domain.Aggregates.Comment.Events;
using CM.Backend.Domain.Aggregates.Tasting.Events;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.EventHandlers.Helpers;
using CM.Backend.Messaging.Contracts;
using CM.Backend.Persistence.EnumOptions;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using Marten.Events.Projections;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class ActivityRMEventHandler :
        IEventHandler<MessageEnvelope<UserFollowed>>,
        IEventHandler<MessageEnvelope<CommentCreated>>,
        IEventHandler<MessageEnvelope<EntityLiked>>,
        IEventHandler<MessageEnvelope<TastingCreated>>
    {
        private readonly ICommandRouter commandRouter;
        private readonly ITastingRepository tastingRepository;
        private readonly IChampagneRepository champagneRepository;
        private readonly ICommentRepository commentRepository;
        private readonly IUserRepository userRepository;
        private readonly INotificationActivityDuplicateRepository notificationActivityDuplicateRepository;

        private const string Tasting = "Tasting";
        private const string Comment = "Comment";
        
        public ActivityRMEventHandler(ICommandRouter commandRouter, ITastingRepository tastingRepository, IChampagneRepository champagneRepository, ICommentRepository commentRepository, IUserRepository userRepository, IFollowRepository followRepository, INotificationActivityDuplicateRepository notificationActivityDuplicateRepository)
        {
            this.commandRouter = commandRouter;
            this.tastingRepository = tastingRepository;
            this.champagneRepository = champagneRepository;
            this.commentRepository = commentRepository;
            this.userRepository = userRepository;
            this.notificationActivityDuplicateRepository = notificationActivityDuplicateRepository;
            
        }

        public async Task HandleAsync(MessageEnvelope<UserFollowed> evt, CancellationToken ct)
        {
            if (evt.Event.Id.Equals(evt.Event.FollowToId))
            {
                return;
            }

            var result = await ResolveDuplicateUserFollowedActivity(evt.Event);

            if (result)
            {
                await commandRouter.RouteAsync<UserFollowedNotification, Response>(new UserFollowedNotification(evt.Event.Id, evt.Event.FollowToId.Value, evt.Event.FollowByName.Value, evt.Event.FollowByImageId.Value));
            } 
        }

        public async Task HandleAsync(MessageEnvelope<CommentCreated> evt, CancellationToken ct)
        {
            var comment = evt.Event;

            var tasting = await tastingRepository.GetById(comment.ContextId.Value);

            var champagne = await champagneRepository.GetById(tasting.ChampagneId);

            var champagneName = ResolveChampagneName(champagne);

            if (!comment.AuthorId.Value.Equals(tasting.AuthorId))
            {
                await commandRouter.RouteAsync<TastingCommentedNotification, Response>(
                    new TastingCommentedNotification(comment.ContextId.Value, tasting.AuthorId, comment.AuthorId.Value, comment.AuthorName.Value,
                        comment.AuthorProfileImgId.Value, champagneName, ContextUrlProvider.ResolveContextUrlForCommentOnTasting(tasting.Id, champagne.Id)));
            }
            else
            {
                await commandRouter.RouteAsync<ActivityInCommentThreadNotification, Response>(
                    new ActivityInCommentThreadNotification(comment.ContextId.Value, tasting.AuthorId, comment.AuthorId.Value,
                        comment.AuthorName.Value,
                        comment.AuthorProfileImgId.Value, champagneName,
                        ContextUrlProvider.ResolveContextUrlForCommentOnTasting(tasting.Id, champagne.Id)));
            }
        }

        public async Task HandleAsync(MessageEnvelope<EntityLiked> evt, CancellationToken ct)
        {
            var like = evt.Event;

            var result = await ResolveDuplicateEntityLikedActivity(evt.Event);

            if (!result)
            {
                return;
            }
            
            if (like.ContextType.Value.Equals(Tasting))
            {
                var tasting = await tastingRepository.GetById(like.LikeToContextId.Value);

                if (evt.Event.Id.Equals(tasting.AuthorId))
                {
                    return;
                }
                
                var champagne = await champagneRepository.GetById(tasting.ChampagneId);

                var champagneName = ResolveChampagneName(champagne);

                await commandRouter.RouteAsync<TastingLikedNotification, Response>(
                    new TastingLikedNotification(tasting.Id, tasting.AuthorId, like.Id, like.LikeByName.Value,
                        like.LikeByProfileImgId.Value, champagneName,
                        ContextUrlProvider.ResolveContextUrlForTastingLiked(tasting.Id, champagne.Id)));
            }

            if (like.ContextType.Value.Equals(Comment))
            {
                var comment = await commentRepository.GetById(like.LikeToContextId.Value);

                if (evt.Id.Equals(comment.AuthorId))
                {
                    return;
                }
                
                var tasting = await tastingRepository.GetById(comment.ContextId);

                var champagne = await champagneRepository.GetById(tasting.ChampagneId);

                var champagneName = ResolveChampagneName(champagne);

                await commandRouter.RouteAsync<CommentLikedNotification, Response>(
                    new CommentLikedNotification(tasting.Id, comment.Id, comment.AuthorId, like.Id, like.LikeByName.Value,
                        like.LikeByProfileImgId.Value, champagneName,
                        ContextUrlProvider.ResolveContextUrlForCommentLiked(champagne.Id, tasting.Id, comment.Id)));
            }
        }

        private string ResolveChampagneName(Champagne champagne)
        {
            var champagneName = champagne.BottleName;

            if (champagne.vintageInfo.IsVintage)
            {
                champagneName += " ";
                champagneName += champagne.vintageInfo.Year;
            }

            return champagneName;
        }

        public async Task HandleAsync(MessageEnvelope<TastingCreated> evt, CancellationToken ct)
        {
            var champagne = await champagneRepository.GetById(evt.Event.ChampagneId.Value);
            var user = await userRepository.GetById(evt.Event.AuthorId.Value);
            
            var champagneName = ResolveChampagneName(champagne);

            await commandRouter.RouteAsync<ChampagneTastedNotification, Response>(
                new ChampagneTastedNotification(user.Id, user.Name, user.ImageCustomization.ProfilePictureImgId,
                    champagneName, ContextUrlProvider.ResolveContextUrlForTastingCreated(champagne.Id, evt.Id)));
        }


        private async Task<bool> ResolveDuplicateUserFollowedActivity(UserFollowed evt)
        {
            //Check notification repository for any duplicate activity.
            var result = await notificationActivityDuplicateRepository.GetByKey(
                new NotificationActivityDuplicate.PrimaryKey {InvokedById = evt.Id, InvokedOnId = evt.FollowToId.Value});

            if (result != null) //The user activity is a duplicate
            {
                return false;
            }

            await notificationActivityDuplicateRepository.Insert(new NotificationActivityDuplicate
            {
                Id = Guid.NewGuid(),
                Key = new NotificationActivityDuplicate.PrimaryKey {InvokedById = evt.Id, InvokedOnId = evt.FollowToId.Value},
                NotificationMethod = NotificationMethod.UserFollowed
            });

            return true;

        }

        private async Task<bool> ResolveDuplicateEntityLikedActivity(EntityLiked evt)
        {
            var result = await notificationActivityDuplicateRepository.GetByKey(
                new NotificationActivityDuplicate.PrimaryKey {InvokedById = evt.Id, InvokedOnId = evt.LikeToContextId.Value});

            if (result != null)
            {
                return false;
            }

            NotificationMethod invokedByMethod = NotificationMethod.Unknown;
            if (evt.ContextType.Value.Equals(Tasting))
            {
                invokedByMethod = NotificationMethod.TastingLiked;
            }

            if (evt.ContextType.Value.Equals(Comment))
            {
                invokedByMethod = NotificationMethod.CommentLiked;
            }
            
            await notificationActivityDuplicateRepository.Insert(new NotificationActivityDuplicate
            {
                Id = Guid.NewGuid(),
                Key = new NotificationActivityDuplicate.PrimaryKey { InvokedById = evt.Id, InvokedOnId = evt.LikeToContextId.Value},
                NotificationMethod = invokedByMethod
            });

            return true;
        }
    }
}