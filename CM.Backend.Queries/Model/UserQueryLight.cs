using System;
namespace CM.Backend.Queries.Model
{
	public class UserQueryLight
    {
		public Guid Id { get; set; }
        public string Name { get; set; }
        public string ProfileName { get; set; }

		public Guid ProfilePictureImgId { get; set; }    
               
    }
}
