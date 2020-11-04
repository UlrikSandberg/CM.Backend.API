using System;
namespace CM.Backend.API.RequestModels
{
    public class EditSectionRequest
    {

		public string Title { get; set; }
	    public string SubTitle { get; set; }
	    public string Body { get; set; }
		public Guid[] Images { get; set; }
		public Guid[] Champagnes { get; set; }

    }
}
