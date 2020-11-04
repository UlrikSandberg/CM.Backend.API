using System;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.Brand.Commands
{
    public class UpdateCellar
    {
		public ImageId CardImageId { get; private set; }
		public ImageId CoverImageId { get; private set; }

		public UpdateCellar(ImageId cardImageId, ImageId coverImageId)
        {
	        if (cardImageId == null || coverImageId == null)
	        {
		        throw new ArgumentException(nameof(UpdateCellar) + ": UpdateCellar(ImageId cardImageId, ImageId coverImageId) --> One or both of the constructor parameters are null which is not valid in the given context");
	        }
	        
            CoverImageId = coverImageId;
			CardImageId = cardImageId;
		}
    }
}
