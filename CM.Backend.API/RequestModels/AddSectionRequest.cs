using System;
namespace CM.Backend.API.RequestModels
{
    public class AddSectionRequest
    {
		public Guid PageId { get; set; }
		public Guid SectionId { get; set; }

    }
}
