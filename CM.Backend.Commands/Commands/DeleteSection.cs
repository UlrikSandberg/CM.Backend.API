using System;
namespace CM.Backend.Commands.Commands
{
	public class DeleteSection : Command
    {
		public Guid BrandId { get; private set; }
		public Guid BrandPageId { get; private set; }
		public Guid SectionId { get; private set; }

		public DeleteSection(Guid brandId, Guid brandPageId ,Guid sectionId)
        {
			BrandId = brandId;
			BrandPageId = brandPageId;
			SectionId = sectionId;
        }
    }
}
