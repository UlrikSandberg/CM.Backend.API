using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class EntityLiked : DomainEvent
    {
        public NotEmptyString LikeByName { get; private set; }
        public ImageId LikeByProfileImgId { get; private set; }
        public AggregateId LikeToContextId { get; private set; }
        public LikeContextType ContextType { get; private set; }

        public EntityLiked(Guid id, NotEmptyString likeByName, ImageId likeByProfileImgId, AggregateId likeToContextId, LikeContextType contextType) : base(id)
        {
            LikeByName = likeByName;
            LikeByProfileImgId = likeByProfileImgId;
            LikeToContextId = likeToContextId;
            ContextType = contextType;
        }
    }
}