using System;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandFile.BrandImage.Commands
{
    public class UploadImage 
    {
	    public AggregateId ImageId { get; private set; }
	    public AggregateId BrandId { get; private set; }
	    public ImageName ImageName { get; private set; }
	    public TypeOfBrandImage TypeOfBrandImage { get; private set; }
	    public FileType FileType { get; private set; }


	    public UploadImage(AggregateId imageId, AggregateId brandId, ImageName imageName, TypeOfBrandImage typeOfBrandImage, FileType fileType)
	    {
		    if (imageId == null || brandId == null || imageName == null || typeOfBrandImage == null || fileType == null)
		    {
			    throw new ArgumentException(nameof(UploadImage) + ": Parameter values may not be null");
		    }
		    
		    ImageId = imageId;
		    BrandId = brandId;
		    ImageName = imageName;
		    TypeOfBrandImage = typeOfBrandImage;
		    FileType = fileType;
	    }
    }
}
