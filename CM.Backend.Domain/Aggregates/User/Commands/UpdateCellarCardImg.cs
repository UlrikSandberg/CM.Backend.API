using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateCellarCardImg
    {
		public ImageId CellarCardImgId { get; private set; }

		public UpdateCellarCardImg(ImageId cellarCardImgId)
        {
	        if (cellarCardImgId == null)
	        {
		        throw new ArgumentException(nameof(cellarCardImgId));
	        }
	        
            CellarCardImgId = cellarCardImgId;
		}
    }
}
