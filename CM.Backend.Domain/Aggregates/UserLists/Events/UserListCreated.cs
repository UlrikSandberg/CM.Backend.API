using System;
using System.Collections.Generic;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.UserLists.Entities;
using CM.Backend.Domain.Aggregates.UserLists.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Events
{
    public class UserListCreated : DomainEvent
    {
        public AggregateId AuthorId { get; }
        public ImageId ImageId { get; }
        public NotEmptyString Title { get; }
        public LimitedString Subtitle { get; }
        public LimitedString Description { get; }
        public FeaturedSchedule FeaturedSchedule { get; }
        public UserListContentType ContentType { get; }
        public UserListType ListType { get; }
        public UserListAuthorType AuthorType { get; }
        public OrderedSet<AggregateId> Content { get; }
        public bool IsDeleted { get; }

        public UserListCreated(Guid id, AggregateId authorId, ImageId imageId, NotEmptyString title, LimitedString subtitle, LimitedString description, FeaturedSchedule featuredSchedule, UserListContentType contentType, UserListType listType, UserListAuthorType authorType, OrderedSet<AggregateId> content, bool isDeleted) : base(id)
        {
            AuthorId = authorId;
            ImageId = imageId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
            FeaturedSchedule = featuredSchedule;
            ContentType = contentType;
            ListType = listType;
            AuthorType = authorType;
            Content = content;
            IsDeleted = isDeleted;
        }
    }
}