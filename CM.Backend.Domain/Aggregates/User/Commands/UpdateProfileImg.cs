using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateProfileImg
    {
		public ImageId ProfileImgId { get; private set; }

		public UpdateProfileImg(ImageId profileImgId)
        {
	        if (profileImgId == null)
	        {
		        throw new ArgumentException(nameof(profileImgId));
	        }
	        
            ProfileImgId = profileImgId;
		}
    }
}
