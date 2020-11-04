using System;

namespace CM.Backend.Persistence.Model.Entities
{
    public class ImageCustomization
    {
        public Guid ProfilePictureImgId { get; set; }
        public Guid ProfileCoverImgId { get; set; }
        public Guid CellarCardImgId { get; set; }
        public Guid CellarHeaderImgId { get; set; }
    }
}