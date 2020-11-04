using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class UnlikeEntity : Command
    {
        public Guid LikeById { get; private set; }
        public Guid LikeToContextId { get; private set; }

        public UnlikeEntity(Guid likeById, Guid likeToContextId)
        {
            LikeById = likeById;
            LikeToContextId = likeToContextId;
        }
    }
}