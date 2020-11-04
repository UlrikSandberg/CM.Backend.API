using System;

namespace CM.Backend.Commands.Commands.TastingCommands
{
    public class DeleteTasting : Command
    {
        public Guid TastingId { get; private set; }
        public Guid UserId { get; private set; }

        public DeleteTasting(Guid tastingId, Guid userId)
        {
            TastingId = tastingId;
            UserId = userId;
        }
    }
}