using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateProfileCover
    {
		public ImageId ProfileCoverImgId { get; private set; }

		public UpdateProfileCover(ImageId profileCoverImgId)
        {
	        if (profileCoverImgId == null)
	        {
		        throw new ArgumentException(nameof(profileCoverImgId));
	        }
	        
            ProfileCoverImgId = profileCoverImgId;
		}
    }
}
