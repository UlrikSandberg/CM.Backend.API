using System;
namespace CM.Backend.Queries.Model
{
    public class UserQueryModel
	{
        public Guid Id { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }
        public string ProfileName { get; set; }
        public string Biography { get; set; }

        //List of saved champagnes
        public int BookmarkedChampagnes { get; set; }

        //List of tasted champagnes 
        public int TastedChampagnes { get; set; }

        //List of people whom this user is following
        public int Following { get; set; } //This is kept since it gives access to quick toggles throughout all usecases where we need to check if this user is following them, way faster in memory and takes the load from database calls.
           
	    public int Followers { get; set; }
	    
        //User image customization
        public UserImageCustomization ImageCustomization { get; set; }

        //Users settings
        public UserSettings Settings { get; set; }

        //Inner class
        public class UserImageCustomization
        {
            public Guid ProfilePictureImgId { get; set; }
            public Guid ProfileCoverImgId { get; set; }
            public Guid CellarCardImgId { get; set; }
            public Guid CellarHeaderImgId { get; set; }
        }

        public class UserSettings
        {
            public string CountryCode { get; set; }
            public string Language { get; set; }
            
            public bool IsEmailVerified { get; set; }
        }
    }
}
