using System;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.Commands;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.Events;
using CM.Backend.Domain.Aggregates.BrandFile.BrandImage.ValueObjects;
using CM.Backend.Domain.SharedValueObjects;

namespace CM.Backend.Domain.Aggregates.BrandFile.BrandImage
{
	public class BrandImage : Aggregate
	{
		public AggregateId BrandId { get; private set; }
		public ImageName ImageName { get; private set; }
		public TypeOfBrandImage TypeOfBrandImage { get; private set; }
		public FileType FileType { get; private set; }
        
        public void Execute(UploadImage cmd)
		{
			RaiseEvent(new ImageUploaded(
				cmd.ImageId.Value,
				cmd.BrandId,
				cmd.ImageName,
				cmd.TypeOfBrandImage,
				cmd.FileType

			));
		}

		protected override void RegisterHandlers()
		{
			Handle<ImageUploaded>(evt =>
			{
				Id = evt.Id;
				BrandId = evt.BrandId;
				ImageName = evt.ImageName;
				TypeOfBrandImage = evt.TypeOfBrandImage;
				FileType = evt.FileType;
			});
		}
	}
}
