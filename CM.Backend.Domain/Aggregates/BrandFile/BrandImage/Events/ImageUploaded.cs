using System;
using CM.Backend.Documents.Events;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandFile.BrandImage.Events
{
	public class ImageUploaded : DomainEvent
	{
		public AggregateId BrandId { get; private set; }
		public ImageName ImageName { get; private set; }
		public TypeOfBrandImage TypeOfBrandImage { get; private set; }
		public FileType FileType { get; private set; }

		public ImageUploaded(Guid id, AggregateId brandId, ImageName imageName, TypeOfBrandImage typeOfBrandImage, FileType fileType) : base(id)
		{
			FileType = fileType;
			TypeOfBrandImage = typeOfBrandImage;
			ImageName = imageName;
			BrandId = brandId;
		}
	}
}
