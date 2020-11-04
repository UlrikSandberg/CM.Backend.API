using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.User.Commands
{
    public class UpdateCellarHeaderImg
    {
		public ImageId CellarHeaderImgId { get; private set; }

		public UpdateCellarHeaderImg(ImageId cellarHeaderImgId)
        {
	        if (cellarHeaderImgId == null)
	        {
		        throw new ArgumentException(nameof(cellarHeaderImgId));
	        }
	        
            CellarHeaderImgId = cellarHeaderImgId;
		}
    }
}
