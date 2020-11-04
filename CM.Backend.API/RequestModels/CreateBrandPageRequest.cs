//using CM.Backend.API.EntityTypeOfRegistry;
using System;


namespace CM.Backend.API.RequestModels
{
	public class CreateBrandPageRequest
	{

		public string Title { get; set; }

		public Guid CardImgId { get; set; }

		public Guid HeaderImgId { get; set; }

	}
}