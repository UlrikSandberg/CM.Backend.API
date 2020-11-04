using System;
namespace CM.Backend.Commands.Commands
{
	public class EditSection : CommandWithIdResponse
    {
		public string Title { get; set; }
	    public string SubTitle { get; private set; }
	    public string Body { get; set; }
		public Guid[] ImageIds { get; set; }
		public Guid[] ChampagneIds { get; set; }
		public Guid SectionId { get; set; }

		public EditSection(Guid sectionId, string title, string subTitle, string body, Guid[] imageIds, Guid[] champagneIds)
        {
            SectionId = sectionId;
			ChampagneIds = champagneIds;
			ImageIds = imageIds;
			Body = body;
			Title = title;
	        SubTitle = subTitle;
        }
    }
}
