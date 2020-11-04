using System;

namespace CM.Backend.Commands.Commands
{
	public class UploadBrandImage : CommandWithIdResponse
    {

		public Guid BrandId { get; private set; }
		public Guid ImageId { get; private set; }
        public string ImageName { get; private set; }
		public string TypeOfBrandImage { get; private set; }
		public string FileExtension { get; private set; }

		public UploadBrandImage(Guid imageId, Guid brandId, string imageName, string typeOfBrandImage, string fileExtension)
        {
			ImageName = imageName;
			BrandId = brandId;
			ImageId = imageId;
			TypeOfBrandImage = typeOfBrandImage;
			FileExtension = fileExtension;
        }
    }
}
