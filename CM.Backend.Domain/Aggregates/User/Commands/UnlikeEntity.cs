using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UnlikeEntity
    {
        public AggregateId LikeToContextId { get; private set; }

        public UnlikeEntity(AggregateId likeToContextId)
        {
            if (likeToContextId == null)
            {
                throw new ArgumentException(nameof(likeToContextId));
            }
            
            LikeToContextId = likeToContextId;
        }
    }
}