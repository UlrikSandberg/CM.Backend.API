using System;
namespace CM.Backend.Commands.Commands
{
	public class CreateAndAddSection : CommandWithIdResponse
    {

		public Guid BrandId { get; private set; }
		public Guid BrandPageId { get; private set; }
		public string Title { get; private set; }
	    public string SubTitle { get; private set; }
	    public string Body { get; private set; }
		public Guid[] Champagnes { get; private set; }
		public Guid[] Images { get; private set; }

		public CreateAndAddSection(Guid brandId,Guid brandPageId, string title, string subTitle, string body, Guid[] champagnes, Guid[] images)
        {

			BrandId = brandId;
			BrandPageId = brandPageId;
			Title = title;
	        SubTitle = subTitle;
	        Body = body;
			Champagnes = champagnes;
			Images = images;

        }
    }
}
