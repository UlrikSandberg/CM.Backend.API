using System;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Documents.Messages;
using CM.Backend.Domain.Aggregates.User.Events;
using CM.Backend.Persistence.Model;
using CM.Backend.Persistence.Repositories;
using SimpleSoft.Mediator;

namespace CM.Backend.EventHandlers
{
    public class LikeRMEventHandler :
        IEventHandler<MessageEnvelope<EntityLiked>>,
        IEventHandler<MessageEnvelope<EntityUnliked>>,
        IEventHandler<MessageEnvelope<UserInfoEdited>>,
        IEventHandler<MessageEnvelope<UserImageCustomizationEdited>>
    {
        private readonly ILikeRepository likeRepository;


        public LikeRMEventHandler(ILikeRepository likeRepository)
        {
            this.likeRepository = likeRepository;
        }


        public async Task HandleAsync(MessageEnvelope<EntityLiked> evt, CancellationToken ct)
        {
            await likeRepository.Insert(new Like
            {
                Id = Guid.NewGuid(),
                Key = new Like.PrimaryKey { LikeById = evt.Event.Id, LikeToContextId = evt.Event.LikeToContextId.Value},
                LikeById = evt.Id,
                LikeByName = evt.Event.LikeByName.Value,
                LikeByProfileImgId = evt.Event.LikeByProfileImgId.Value,
                LikeToContextId = evt.Event.LikeToContextId.Value,
                ContextType = evt.Event.ContextType.Value
            });
        }

        public async Task HandleAsync(MessageEnvelope<EntityUnliked> evt, CancellationToken ct)
        {
            var key = new Like.PrimaryKey {LikeById = evt.Id, LikeToContextId = evt.Event.UnlikeToContextId.Value};

            await likeRepository.DeleteLike(key);
        }

        public async Task HandleAsync(MessageEnvelope<UserInfoEdited> evt, CancellationToken ct)
        {
            if (evt.Event.DidNameChange)
            {
                await likeRepository.UpdateLikeByNameBatchAsync(evt.Id, evt.Event.Name.Value);
            }
        }

        public async Task HandleAsync(MessageEnvelope<UserImageCustomizationEdited> evt, CancellationToken ct)
        {
            if (evt.Event.ProfileImageChanged)
            {
                await likeRepository.UpdateLikeByProfileImageBatchAsync(evt.Id, evt.Event.ProfileImageId.Value);
            }
        }
    }
}