using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateUserImageCustomization
    {
        public ImageId ProfileImageId { get; set; }
        public ImageId ProfileCoverImageId { get; set; }
        public ImageId ProfileCellarCardImageId { get; set; }
        public ImageId ProfileCellarHeaderImageId { get; private set; }

        public UpdateUserImageCustomization(ImageId profileImageId, ImageId profileCoverImageId, ImageId profileCellarCardImageId, ImageId profileCellarHeaderImageId)
        {
            if (profileImageId == null || profileCoverImageId == null || profileCellarCardImageId == null ||
                profileCellarHeaderImageId == null)
            {
                throw new ArgumentException(nameof(UpdateUserImageCustomization) + ": Parameter values may not be null");
            }
            
            ProfileImageId = profileImageId;
            ProfileCoverImageId = profileCoverImageId;
            ProfileCellarCardImageId = profileCellarCardImageId;
            ProfileCellarHeaderImageId = profileCellarHeaderImageId;
        }
    }
}