using System;
namespace CM.Backend.Commands.Commands
{
	public class CreateSection : CommandWithIdResponse
    {
		public Guid BrandId { get; set; }
		public string Title { get; set; }
	    public string SubTitle { get; private set; }
	    public string Body { get; set; }
		public Guid[] Champagnes { get; set; }
		public Guid[] Images { get; set; }

		public CreateSection(Guid brandId, string title, string subTitle, string body, Guid[] champagnes, Guid[] images)
        {
            Images = images;
			Champagnes = champagnes;
			Body = body;
			Title = title;
	        SubTitle = subTitle;
	        BrandId = brandId;
		}
    }
}
