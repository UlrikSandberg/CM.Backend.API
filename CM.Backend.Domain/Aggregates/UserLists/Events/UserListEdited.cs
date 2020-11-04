using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class UserListEdited : DomainEvent
    {
        public ImageId ImageId { get; }
        public NotEmptyString Title { get; }
        public LimitedString Subtitle { get; }
        public LimitedString Description { get; }

        public UserListEdited(Guid id, ImageId imageId, NotEmptyString title, LimitedString subtitle, LimitedString description) : base(id)
        {
            ImageId = imageId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
        }
    }
}