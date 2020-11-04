using Microsoft.AspNetCore.Http;

namespace CM.Backend.API.RequestModels.UserRequestModels
{
    public class UpdateUserSettingsRequestModel
    {
        public string Name { get; set; }
        public string ProfileName { get; set; }
        public string Biography { get; set; }
        
        public byte[] ProfileImage { get; set; }
        public byte[] ProfileCoverImage { get; set; }
        public byte[] ProfileCellarCardImage { get; set; }
    }
}