using System.Collections.Generic;

namespace CM.Backend.API.RequestModels
{
	public class AddChampagneFilterSearchOptionRequest
    {

		public string Dosage { get; set; }
		public List<string> Styles { get; set; }
		public List<string> Characters { get; set; }
    }
}
