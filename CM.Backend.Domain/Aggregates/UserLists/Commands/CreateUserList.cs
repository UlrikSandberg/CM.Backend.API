using System;
using System.Collections.Generic;
using CM.Backend.Domain.Aggregates.UserLists.Entities;
using CM.Backend.Domain.Aggregates.UserLists.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.UserLists.Commands
{
    public class CreateUserList
    {
        public AggregateId Id { get; }
        public AggregateId AuthorId { get; }
        public ImageId ImageId { get; }
        public NotEmptyString Title { get; }
        public LimitedString Subtitle { get; }
        public LimitedString Description { get; }
        public FeaturedSchedule FeaturedSchedule { get; }
        public UserListContentType ContentType { get; }
        public UserListType ListType { get; }
        public OrderedSet<AggregateId> ListContent { get; }
        public UserListAuthorType AuthorType { get; }

        public bool IsDeleted { get; } = false;

        public CreateUserList(AggregateId id, AggregateId authorId, ImageId imageId, NotEmptyString title,
            LimitedString subtitle, LimitedString description, FeaturedSchedule featuredTimeSpan,
            UserListContentType contentType, UserListType listType, OrderedSet<AggregateId> listContent,
            UserListAuthorType authorType)
        {
            Id = id;
            AuthorId = authorId;
            ImageId = imageId;
            Title = title;
            Subtitle = subtitle;
            Description = description;
            FeaturedSchedule = FeaturedSchedule;
            ContentType = contentType;
            ListType = listType;
            ListContent = listContent;
            AuthorType = authorType;

            //Loop over each property and check for null condition
            foreach (var property in GetType().GetProperties())
            {
                if (property.GetValue(this) == null)
                {
                    throw new ArgumentException($"One or more values in {nameof(CreateUserList)} constructor is null");
                }
            }
        }
    }
}