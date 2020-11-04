using System;
using CM.Backend.Domain.Aggregates.User.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class LikeEntity
    {
        public AggregateId LikeToContextId { get; private set; }
        public LikeContextType ContextType { get; private set; }

        public LikeEntity(AggregateId likeToContextId, LikeContextType contextType)
        {
            if (likeToContextId == null || contextType == null)
            {
                throw new ArgumentException(nameof(LikeEntity) + ": Parameter values may not be null");
            }
            
            LikeToContextId = likeToContextId;
            ContextType = contextType;
        }
    }
}