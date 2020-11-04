using System;
namespace CM.Backend.Commands.Commands
{
	public class AddSection : Command
    {
		public Guid BrandId { get; private set; }
		public Guid PageId { get; private set; }
		public Guid SectionId { get; private set; }

		public AddSection(Guid brandId, Guid pageId, Guid sectionId)
        {
			BrandId = brandId;
			SectionId = sectionId;
			PageId = pageId;
		}
    }
}
