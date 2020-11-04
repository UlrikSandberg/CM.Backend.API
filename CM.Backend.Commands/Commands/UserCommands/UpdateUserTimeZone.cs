using System;

namespace CM.Backend.Commands.Commands.UserCommands
{
    public class UpdateUserTimeZone : Command
    {
        public Guid UserId { get; private set; }
        public long UTCOffset { get; private set; }

        public UpdateUserTimeZone(Guid userId, long utcOffset)
        {
            UserId = userId;
            UTCOffset = utcOffset;
        }
    }
}