using System;
namespace CM.Backend.API.RequestModels
{
    public class EditBrandPageRequest
	{
		public string Title { get; set; }
		public Guid CardImgId { get; set; }
		public Guid HeaderImgId { get; set; }

		public EditBrandPageRequest(string title, Guid cardImgId, Guid headerImgId)
        {
            HeaderImgId = headerImgId;
			CardImgId = cardImgId;
			Title = title;
		}
    }
}
