using System;

namespace CM.Backend.Queries.Model
{
    public class PublicUserQueryModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string ProfileName { get; set; }
        public string Biography { get; set; }
        
        public int BookmarkedChampagnes { get; set; }
        
        public int TastedChampagnes { get; set; }
        
        public int Following { get; set; }
        
        public int Followers { get; set; }

        public UserImageCustomization imageCustomization { get; set; }
        
        public bool IsRequesterFollowing { get; set; }
        
        public class UserImageCustomization
        {
            public Guid ProfilePictureImgId { get; set; }
            public Guid ProfileCoverImgId { get; set; }
            public Guid CellarCardImgId { get; set; }
            public Guid CellarHeaderImgId { get; set; }
        }
    }
}