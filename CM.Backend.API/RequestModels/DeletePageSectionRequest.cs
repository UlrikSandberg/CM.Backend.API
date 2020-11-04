using System;
namespace CM.Backend.API.RequestModels
{
    public class DeletePageSectionRequest
    {

		public Guid BrandId { get; set; }
		public Guid BrandPageId { get; set; }
		public Guid SectionId { get; set; }

		public DeletePageSectionRequest(Guid brandId, Guid brandPageId, Guid sectionId)
        {
			BrandId = brandId;
			BrandPageId = brandPageId;
			SectionId = sectionId;
        }
    }
}
