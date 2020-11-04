using System;
namespace CM.Backend.Commands.Commands
{
	public class RemoveSection : Command
    {
		public Guid BrandId { get; private set; }
		public Guid PageId { get; private set; }
		public Guid SectionId { get; private set; }

		public RemoveSection(Guid brandId, Guid pageId, Guid sectionId)
        {
            SectionId = sectionId;
			PageId = pageId;
			BrandId = brandId;
		}
    }
}
