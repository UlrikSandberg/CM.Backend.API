using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Events
{
    public class UserImageCustomizationEdited : DomainEvent
    {
        public ImageId ProfileImageId { get; private set; }
        public ImageId ProfileCoverImageId { get; private set; }
        public ImageId ProfileCellarCardImageId { get; private set; }
        public ImageId ProfileCellarHeaderImageId { get; private set; }
        public bool ProfileImageChanged { get; private set; }

        public UserImageCustomizationEdited(Guid id, ImageId profileImageId, ImageId profileCoverImageId, ImageId profileCellarCardImageId, ImageId profileCellarHeaderImageId, bool profileImageChanged) : base(id)
        {
            ProfileImageId = profileImageId;
            ProfileCoverImageId = profileCoverImageId;
            ProfileCellarCardImageId = profileCellarCardImageId;
            ProfileCellarHeaderImageId = profileCellarHeaderImageId;
            ProfileImageChanged = profileImageChanged;
        }
    }
}