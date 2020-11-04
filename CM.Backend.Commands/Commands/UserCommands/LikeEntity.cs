using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class LikeEntity : Command
    {
        public Guid LikeById { get; private set; }
        public Guid LikeToContextId { get; private set; }
        public string ContextType { get; private set; }

        public LikeEntity(Guid likeById, Guid likeToContextId, string contextType)
        {
            LikeById = likeById;
            LikeToContextId = likeToContextId;
            ContextType = contextType;
        }
    }
}