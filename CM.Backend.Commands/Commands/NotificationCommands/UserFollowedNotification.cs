using System;

namespace CM.Backend.Commands.Commands.NotificationCommands
{
    public class UserFollowedNotification : Command
    {
        public Guid FollowById { get; private set; }
        public Guid FollowToId { get; private set; }
        public string FollowByName { get; private set; }
        public Guid FollowByImageId { get; private set; }

        public UserFollowedNotification(Guid followById, Guid followToId, string followByName, Guid followByImageId)
        {
            FollowById = followById;
            FollowToId = followToId;
            FollowByName = followByName;
            FollowByImageId = followByImageId;
        }
    }
}