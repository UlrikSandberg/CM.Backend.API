using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Entities
{
    public class UserImageCustomization
    {
		public ImageId ProfilePictureImgId { get; set; }
        public ImageId ProfileCoverImgId { get; set; }
        public ImageId CellarCardImgId { get; set; }
        public ImageId CellarHeaderImgId { get; set; }
    }
}
